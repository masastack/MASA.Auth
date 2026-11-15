// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using StackExchange.Redis;

namespace Masa.Auth.Service.Admin.Services;

/// <summary>
/// 在线用户与踢人（IS4 grant-first 设计）。
/// 会话事实源 = auth.PersistedGrant 表：客户端会话 = 未过期且未消费的 refresh_token 行；
/// 门户会话 = SSO 自写的 sso_session 行（见 OnlineUserConsts）。
/// "最近活跃"取组内 MAX(COALESCE(ConsumedTime, CreationTime))——IS4 轮换续期复用同一个
/// RefreshToken 对象、沿用原 CreationTime 落库，因此 CreationTime 恒等于登录时刻，
/// 真正的续期时间写在已消费旧行的 ConsumedTime 上（详见 BuildOnlineUsersAsync）。
/// 踢人 = 直接删行 + 水位线 + Stream 通知 SSO 发 backchannel。
/// </summary>
public class OnlineUserService : ServiceBase
{
    /// <summary>服务基路径。踢人权限码由请求路径推导（见 <see cref="KickUserAsync"/>），
    /// 故 <c>[MasaAuthorize]</c> 不能显式写 Code（不会补 <c>{appId}.</c> 前缀，永远匹配不上），
    /// 改路径也必须同步改种子权限码，<c>OnlineUserAuthorizationTests</c> 守着这些约束</summary>
    internal const string BaseRoute = "api/online-user";

    internal const string KickRoute = "kick";

    public OnlineUserService() : base(BaseRoute)
    {
        // 鉴权一律用处理方法上的特性，不要依赖 RouteHandlerBuilder：Masa 只在 AutoMapRoute 里读取它，
        // 而 Program.cs 开了 DisableAutoMapRoute，显式的 MapGet/MapPost 完全不会应用它（实测 endpoint 元数据里没有授权信息）。
        //   [Authorize]       = 仅需登录（列表 / 会话查询，与其他列表接口的口径一致）
        //   [MasaAuthorize]   = 登录 + 权限码（踢人，与 TeamService.CreateAsync 同一套机制）
        MapGet(GetListAsync);
        MapGet(GetSessionsAsync, "{subjectId}/sessions");
        MapPost(KickUserAsync, KickRoute);
    }

    #region List

    [Authorize]
    public async Task<IResult> GetListAsync(
        [FromServices] IEventBus eventBus,
        [FromServices] AuthDbContext dbContext,
        [FromServices] OidcDbContext oidcDbContext,
        [FromServices] IMasaConfiguration masaConfiguration,
        [FromServices] ILogger<OnlineUserService> logger,
        GetOnlineUsersDto input)
    {
        var thresholdMinutes = masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue("OnlineUser:OnlineThresholdMinutes", OnlineUserConsts.DefaultOnlineThresholdMinutes);

        // 渠道过滤作用在会话行上（而不是事后筛用户）：返回的 Sessions / SessionCount / IsOnline 都只按该渠道计算，
        // 避免 Channel=App 的结果里仍混着该用户的 Web 会话。非法值视为不过滤
        var channel = input.Channel is OnlineUserConsts.ChannelWeb or OnlineUserConsts.ChannelApp ? input.Channel : null;

        var users = await BuildOnlineUsersAsync(dbContext, OidcAsDbContext(oidcDbContext), eventBus,
            DateTime.UtcNow, TimeSpan.FromMinutes(thresholdMinutes), channelFilter: channel, onlyOnline: input.OnlyOnline,
            logger: logger);

        if (!string.IsNullOrWhiteSpace(input.Search))
        {
            var search = input.Search.Trim();
            users = users.Where(u =>
                u.Account.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                u.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        var total = users.Count;
        var items = users
            .Skip((input.Page - 1) * input.PageSize)
            .Take(input.PageSize)
            .ToList();

        return Results.Ok(new PaginationDto<OnlineUserDto>(total, items));
    }

    [Authorize]
    public async Task<IResult> GetSessionsAsync(
        [FromServices] IEventBus eventBus,
        [FromServices] AuthDbContext dbContext,
        [FromServices] OidcDbContext oidcDbContext,
        [FromServices] IMasaConfiguration masaConfiguration,
        [FromServices] ILogger<OnlineUserService> logger,
        string subjectId)
    {
        var thresholdMinutes = masaConfiguration.ConfigurationApi.GetDefault()
            .GetValue("OnlineUser:OnlineThresholdMinutes", OnlineUserConsts.DefaultOnlineThresholdMinutes);

        // 会话明细接口始终返回该用户的全部渠道会话（含已超阈值、IsOnline=false 的），便于排查与精确踢人
        var users = await BuildOnlineUsersAsync(dbContext, OidcAsDbContext(oidcDbContext), eventBus,
            DateTime.UtcNow, TimeSpan.FromMinutes(thresholdMinutes), subjectId, logger: logger);

        var user = users.FirstOrDefault();
        return user == null
            ? Results.Ok(Array.Empty<UserSessionDto>())
            : Results.Ok(user.Sessions);
    }

    private record GrantRow(
        string SubjectId, string Type, string ClientId, string? SessionId, string Key,
        DateTime CreationTime, DateTime? ConsumedTime);

    /// <summary>渠道判定：SessionId 非空 = Web（浏览器 code 流），为空 = App（token-only 流）</summary>
    internal static string ResolveChannel(string? sessionId)
        => sessionId is null ? OnlineUserConsts.ChannelApp : OnlineUserConsts.ChannelWeb;

    // Masa 的 OidcDbContext 是包装类，隐式转换为 EF DbContext 后才能 Set<T>()（同 Application/Sso/QueryHandler 模式）
    private static DbContext OidcAsDbContext(OidcDbContext oidcDbContext) => oidcDbContext;

    /// <summary>在线阈值与客户端 AT 寿命的联动系数：续期（refresh）只在 AT 过期时发生，所以两次续期的
    /// 最大间隔 = AT 寿命；阈值低于 2 × AT 时，正常使用中的用户会在下一次续期之前被误判离线
    /// （缺省值 120 分钟 = 2 × IS4 默认 AT 3600s 正是按这个公式取的，见 OnlineUserConsts）</summary>
    internal const int OnlineThresholdToAccessTokenRatio = 2;

    /// <summary>按租户内启用客户端的最大 AccessTokenLifetime（秒）推导安全的在线阈值（分钟），向上取整。
    /// 阈值（DCC OnlineUser:OnlineThresholdMinutes）配得比它小就要准备看到"活跃用户显示离线"</summary>
    internal static int SuggestedOnlineThresholdMinutes(int maxEnabledAccessTokenSeconds)
        => (OnlineThresholdToAccessTokenRatio * maxEnabledAccessTokenSeconds + 59) / 60;

    /// <summary>阈值是否低于联动下限（纯函数，测试锚定）</summary>
    internal static bool ThresholdTooSmall(int thresholdMinutes, int maxEnabledAccessTokenSeconds)
        => thresholdMinutes < SuggestedOnlineThresholdMinutes(maxEnabledAccessTokenSeconds);

    /// <summary>阈值联动告警的节流状态：Ticks（UnixEpoch 起算）</summary>
    static long _lastThresholdWarningTicks;

    /// <summary>
    /// 在线阈值联动告警：配置的阈值低于 2 × 最大启用客户端 AT 寿命时 LogWarning，提示调大
    /// DCC OnlineUser:OnlineThresholdMinutes。每小时最多一条（阈值与客户端 AT 都可以热更，
    /// 只告一次会漏掉后续的配置漂移）；比较逻辑是纯函数，告警与否不改变在线裁剪行为。
    /// 并发下最多多打一条日志，可接受，不做重量级同步。
    /// </summary>
    static void WarnThresholdTooSmall(ILogger? logger, TimeSpan onlineThreshold, int maxEnabledAccessTokenSeconds)
    {
        if (logger == null)
        {
            return;
        }

        var thresholdMinutes = (int)Math.Ceiling(onlineThreshold.TotalMinutes);
        if (!ThresholdTooSmall(thresholdMinutes, maxEnabledAccessTokenSeconds))
        {
            return;
        }

        var nowTicks = DateTime.UtcNow.Ticks;
        if (nowTicks - Interlocked.Read(ref _lastThresholdWarningTicks) < TimeSpan.FromHours(1).Ticks)
        {
            return;
        }

        Interlocked.Exchange(ref _lastThresholdWarningTicks, nowTicks);
        logger.LogWarning(
            "Online threshold ({ThresholdMinutes} min) is below {Ratio} x the max enabled client AccessTokenLifetime ({MaxAtSeconds} s): " +
            "actively used sessions can be misjudged as offline before their next refresh. " +
            "Raise DCC 'OnlineUser:OnlineThresholdMinutes' to at least {SuggestedMinutes} min.",
            thresholdMinutes, OnlineThresholdToAccessTokenRatio, maxEnabledAccessTokenSeconds,
            SuggestedOnlineThresholdMinutes(maxEnabledAccessTokenSeconds));
    }


    /// <summary>
    /// 会话事实源的读取与聚合。
    ///
    /// 关键点：IS4 轮换续期**复用同一个 RefreshToken 对象**（只改 Lifetime / ConsumedTime），
    /// 落库时沿用反序列化出来的原始 CreationTime（DefaultGrantStore.CreateItemAsync）。
    /// 于是同一条续期链上所有 refresh_token 行的 CreationTime 恒等于登录时刻——它既不是
    /// "最近活跃"也不是"最近一次建 token"。真正的活跃信号在**已消费的旧行**上：
    /// 每次轮换都会给旧行写 ConsumedTime = 轮换时刻。
    ///
    /// 因此这里：
    /// - 会话存活：组内存在 ConsumedTime == null 的行（否则该续期链已终态，不再展示）；
    /// - 最近续期：组内 MAX(COALESCE(ConsumedTime, CreationTime))，包含已消费行；
    /// - 从未续期的会话退化为 CreationTime（登录时刻），这是唯一可用的近似。
    ///
    /// 已知限制（见 online-user-kick_plan.md）：客户端配置 RefreshTokenUsage=ReUse 时不轮换、
    /// 不产生 ConsumedTime，活跃时间退化为登录时刻；纯门户用户（无 offline_access）同理。
    /// </summary>
    internal static async Task<List<OnlineUserDto>> BuildOnlineUsersAsync(
        AuthDbContext dbContext,
        DbContext oidcDbContext,
        IEventBus eventBus,
        DateTime now,
        TimeSpan onlineThreshold,
        string? subjectIdFilter = null,
        string? channelFilter = null,
        bool onlyOnline = false,
        ILogger? logger = null)
    {
        var renewedAfter = now - onlineThreshold;

        // 不再过滤 ConsumedTime：已消费行携带续期时间，是聚合"最近续期"的输入
        var grantsQuery = dbContext.Set<PersistedGrant>().AsNoTracking()
            .Where(g => (g.Type == OnlineUserConsts.RefreshTokenType || g.Type == OnlineUserConsts.PortalSessionType)
                     && g.Expiration > now);
        if (subjectIdFilter != null)
        {
            grantsQuery = grantsQuery.Where(g => g.SubjectId == subjectIdFilter);
        }
        // 渠道 = SessionId 是否为空（与 ResolveChannel 同一规则），必须下推到聚合之前：
        // 否则只统计另一个渠道的用户会因"有存活会话"而被错误保留
        if (channelFilter == OnlineUserConsts.ChannelWeb)
        {
            grantsQuery = grantsQuery.Where(g => g.SessionId != null);
        }
        else if (channelFilter == OnlineUserConsts.ChannelApp)
        {
            grantsQuery = grantsQuery.Where(g => g.SessionId == null);
        }

        // 候选用户：SQL 侧只取 DISTINCT SubjectId，不再对整张表做 GROUP BY 聚合。
        // refresh_token 的绝对寿命默认 30 天，闲置 grant 会长期留在表里：此前的"按用户聚合出存活 + 最近续期"
        // 会为每个有 grant 的用户回传一行（并在库里对近乎全表做聚合），在线人数只占其中一小部分时极不划算，
        // 而且这个接口常被前端轮询。
        //   - onlyOnline（默认）：在线 ⇒ 存在"续期时刻晚于阈值"的行（MAX(COALESCE(ConsumedTime, CreationTime)) > 阈值
        //     等价于存在这样一行），所以只取有近期活动行的用户，规模 ≈ 在线人数；
        //   - 否则（排查"谁还挂着会话"）：有任一存活行（ConsumedTime == null）的用户。
        // 这只是必要条件的粗筛：是否真有存活会话、存活会话的最近续期是否在阈值内，下面载入行之后在内存里精确复核
        // （与此前 SQL 聚合 + 二次裁剪的最终结果一致）。
        var candidateQuery = onlyOnline
            ? grantsQuery.Where(g => (g.ConsumedTime ?? g.CreationTime) > renewedAfter)
            : grantsQuery.Where(g => g.ConsumedTime == null);
        var subjectIds = await candidateQuery
            .Select(g => g.SubjectId)
            .Distinct()
            .ToListAsync();

        if (subjectIds.Count == 0)
        {
            return new List<OnlineUserDto>();
        }

        // 分批取行：subjectIds 是 IN 列表，SqlServer 的语句参数上限是 2100，
        // 万级同时在线时会直接报错。按 1000 一批拼结果即可（每批都是同一谓词的无状态查询）。
        const int chunkSize = 1000;
        var rows = new List<GrantRow>(subjectIds.Count * 2);
        for (var offset = 0; offset < subjectIds.Count; offset += chunkSize)
        {
            var chunk = subjectIds.Skip(offset).Take(chunkSize).ToList();
            rows.AddRange(await grantsQuery
                .Where(g => chunk.Contains(g.SubjectId))
                .Select(g => new GrantRow(g.SubjectId, g.Type, g.ClientId, g.SessionId, g.Key, g.CreationTime, g.ConsumedTime))
                .ToListAsync());
        }

        var clientInfos = await oidcDbContext.Set<Client>().AsNoTracking()
            .Select(c => new { c.ClientId, c.ClientName, c.ClientType, c.Enabled, c.AccessTokenLifetime })
            .ToListAsync();
        var clientMap = clientInfos.GroupBy(c => c.ClientId).ToDictionary(g => g.Key, g => g.First());

        // 阈值联动告警：续期信号间隔上界 = AT 寿命，阈值低于 2 × 最大启用客户端 AT 时，正常使用中的
        // 用户会在下一次续期之前被误判离线（默认 120 = 2 × 3600s；调短 App 客户端 AT 时阈值可随之调小，
        // 反之调大 AT 必须同步调大阈值，见 online-user-kick_plan.md 的 SOP）
        var maxEnabledAtSeconds = clientInfos.Where(c => c.Enabled)
            .Select(c => (int?)c.AccessTokenLifetime).Max() ?? 0;
        WarnThresholdTooSmall(logger, onlineThreshold, maxEnabledAtSeconds);

        // 会话分组键：
        //   Web（sid 非空）按 (sid, clientId)——同一浏览器会话下每个客户端一条；
        //   App（sid 为空）按 (sub, clientId)——单活策略保证只有一条存活行，已消费的旧行并入同一族；
        //   门户行（sso_session）ClientId 是占位值，天然落在 web:{sid}:sso-portal 独立一组。
        List<UserSessionDto> BuildSessions(IEnumerable<GrantRow> rows) => rows
            .Select(r =>
            {
                var isPortal = r.Type == OnlineUserConsts.PortalSessionType;
                var client = clientMap.GetValueOrDefault(r.ClientId);
                return new
                {
                    r.SessionId,
                    r.ClientId,
                    r.Key,
                    r.CreationTime,
                    r.ConsumedTime,
                    Channel = ResolveChannel(r.SessionId),
                    ClientName = isPortal ? "SSO 门户" : client?.ClientName ?? r.ClientId,
                    ClientType = isPortal ? ClientTypes.Web : client?.ClientType ?? ClientTypes.Web
                };
            })
            .GroupBy(r => r.SessionId is null ? $"app:{r.ClientId}" : $"web:{r.SessionId}:{r.ClientId}")
            .Select(g =>
            {
                // 存活行：未被消费。整组都已被消费说明续期链已终结（例如客户端换号/被顶号），
                // 这些行只是历史活跃记录，不再作为会话展示
                var alive = g.FirstOrDefault(x => x.ConsumedTime == null);
                if (alive == null)
                {
                    return null;
                }

                return new UserSessionDto(
                    alive.SessionId,
                    alive.ClientId,
                    alive.ClientName,
                    alive.ClientType,
                    alive.Channel,
                    // GrantKey 必须是**存活行**的 Key：已消费行的 Key 早已不能用于 refresh
                    alive.SessionId is null ? alive.Key : null,
                    g.Min(x => x.CreationTime),
                    g.Max(x => x.ConsumedTime ?? x.CreationTime));
            })
            .Where(s => s != null)
            .Select(s => s!)
            .OrderByDescending(s => s.LastRenewedAt)
            .ToList();

        var sessionDict = rows
            .Where(r => Guid.TryParse(r.SubjectId, out _))
            .GroupBy(r => r.SubjectId)
            .ToDictionary(g => g.Key, BuildSessions);

        // 精确复核（候选用户只是 SQL 的粗筛，见上）：
        // - 没有存活会话（只剩已终结的续期链、或渠道过滤后一个会话都不剩）→ 不出现；
        // - 按**存活会话**的实际最近续期判定在线：候选条件里的"近期活动行"可能来自已终结的续期链
        //   （例如 webapp 链 1 分钟前刚被顶号、只剩 10 小时前的门户行），不能据此放行 onlyOnline。
        var survivors = new List<(string SubjectId, List<UserSessionDto> Sessions, DateTime LastRenewed)>(subjectIds.Count);
        foreach (var subjectId in subjectIds)
        {
            if (!sessionDict.TryGetValue(subjectId, out var userSessions) || userSessions.Count == 0)
            {
                continue;
            }

            var lastRenewed = userSessions.Max(s => s.LastRenewedAt);
            if (onlyOnline && lastRenewed <= renewedAfter)
            {
                continue;
            }

            survivors.Add((subjectId, userSessions, lastRenewed));
        }

        // 画像只取最终会出现在结果里的用户（Search 在内存里按画像过滤，所以要覆盖全部幸存者，而不是只覆盖一页）
        var portraits = new Dictionary<Guid, UserModel>();
        if (survivors.Count > 0)
        {
            var query = new UserPortraitsQuery(survivors.Select(s => Guid.Parse(s.SubjectId)).ToList());
            await eventBus.PublishAsync(query);
            portraits = query.Result.ToDictionary(u => u.Id);
        }

        // 排序：存活会话最近续期者优先；候选集来自 DISTINCT 查询、没有固有顺序，用 UserId 兜底保证结果稳定
        return survivors
            .OrderByDescending(s => s.LastRenewed)
            .ThenBy(s => s.SubjectId, StringComparer.Ordinal)
            .Select(s =>
            {
                var userId = Guid.Parse(s.SubjectId);
                portraits.TryGetValue(userId, out var portrait);
                return new OnlineUserDto(
                    userId,
                    portrait?.Account ?? s.SubjectId,
                    portrait?.DisplayName ?? string.Empty,
                    portrait?.Avatar ?? string.Empty,
                    s.LastRenewed > renewedAfter,
                    s.Sessions.Count,
                    s.Sessions);
            })
            .ToList();
    }

    #endregion

    #region Kick

    /// <summary>
    /// 强制下线：写水位线、Stream 通知 SSO 发 backchannel，最后删除会话行。
    /// </summary>
    [MasaAuthorize]
    public async Task<IResult> KickUserAsync(
        [FromBody] KickOnlineUserModel model,
        [FromServices] IEventBus eventBus,
        [FromServices] AuthDbContext dbContext,
        [FromServices] IConnectionMultiplexer mux,
        [FromServices] OperaterProvider operaterProvider,
        [FromServices] IUserContext userContext,
        [FromServices] ILogger<OnlineUserService> logger)
    {
        var invalid = ValidateKick(model);
        if (invalid != null) return Results.BadRequest(invalid);

        // DB 与 Redis 无法放进同一事务，所以用顺序换可重试性：
        //   快照 → ①水位线 → ②Stream 通知 → ③删行（最后一步）。
        // Redis 两步都是幂等的，失败发生在删行之前时 DB 毫无变化，管理员直接重试即可完整重做；
        // 若反过来先删行，Redis 一旦失败，行已删除，重试时快照为空——sid 水位线与 backchannel 目标都无从重建，
        // 门户 Cookie 与客户端登出就永久丢了。删行失败（极少见）则 Redis 侧已生效，重试会补删并重发通知（客户端处理重复的 logout_token 天然幂等）。
        //
        // 快照（用于水位线 sid、backchannel 目标）。后面按同一过滤条件删除，而不是按快照主键删：
        // 快照之后并发 refresh 新写入的 refresh_token 行也必须一并清掉，否则踢人后仍会残留一条有效 token
        var grants = await FilterForKick(dbContext.Set<PersistedGrant>(), model).AsNoTracking().ToListAsync();

        // 1. 水位线（Admin 同步写，cookie 拦截不依赖 SSO 在线）
        var db = mux.GetDatabase();
        var kickedAtMs = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString();
        var watermarkTtl = TimeSpan.FromDays(OnlineUserConsts.KickedAtKeyTtlDays);
        if (IsKickAll(model))
        {
            // 无条件写（即便没有匹配到任何行）：上线前登录的浏览器没有 sso_session 行，
            // sub 水位线是让这些旧 Cookie 失效的唯一手段
            await db.StringSetAsync(OnlineUserConsts.KickedAtSubjectKey(model.SubjectId), kickedAtMs, watermarkTtl);
        }
        foreach (var sid in GetWatermarkSessionIds(model, grants))
        {
            await db.StringSetAsync(OnlineUserConsts.KickedAtSessionKey(sid), kickedAtMs, watermarkTtl);
        }

        // 2. Stream 通知 SSO 发 backchannel（SSO 不在线时消息滞留，恢复后补发）。
        // 按被删行的 sid 拆分目标：每个浏览器会话各发一次带 sid 的 logout_token，
        // 满足开启 BackChannelLogoutSessionRequired 的客户端（IS4 默认开启）对 sid 的强制要求
        var targets = BuildKickTargets(grants);
        var clientIds = targets.SelectMany(t => t.ClientIds).Distinct().ToList();
        var operatorId = userContext.GetUserId<Guid>();
        if (targets.Count > 0)
        {
            var message = JsonSerializer.Serialize(
                new KickRequestMessage(model.SubjectId, targets, operatorId, DateTime.UtcNow));
            await db.StreamAddAsync(
                OnlineUserConsts.KickRequestStream,
                "message",
                message,
                maxLength: OnlineUserConsts.KickRequestStreamMaxLength,
                useApproximateMaxLength: true);
        }

        // 3. 删行（AuthDbContext 与 SSO 同表，删行即生效——踢人后 refresh 返回 invalid_grant）
        var removedCount = await FilterForKick(dbContext.Set<PersistedGrant>(), model).ExecuteDeleteAsync();

        // 4. 审计（失败不阻断踢人）
        try
        {
            var operatorModel = await operaterProvider.GetUserAsync(operatorId);
            var description = BuildKickDescription(model, operatorModel?.DisplayName, removedCount, clientIds);
            await eventBus.PublishAsync(new AddOperationLogCommand(
                operatorId, operatorModel?.DisplayName ?? string.Empty,
                OperationTypes.SignOut, DateTime.UtcNow, description, null));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to write kick audit log for subject {SubjectId}", model.SubjectId);
        }

        // RemovedGrants 是**删除的 grant 行数**，不等于界面上的"会话数"：包含已消费的轮换旧行
        // 与 authorization_code 等非会话行；也不含 user_consent（踢全部时保留）。
        // 为 0 不代表失败——水位线与通知仍然已生效（例如用户只有门户 Cookie 而没有任何 grant 行）。
        return Results.Ok(new KickOnlineUserResultDto(removedCount, clientIds));
    }

    /// <summary>请求合法性：返回 null 表示通过，否则为 400 的错误信息</summary>
    internal static string? ValidateKick(KickOnlineUserModel model)
    {
        if (string.IsNullOrWhiteSpace(model.SubjectId))
        {
            return "SubjectId is required.";
        }
        // 单独的 ClientId 语义不明（曾会退化成踢该用户全部），必须与 SessionId 一起使用
        if (!string.IsNullOrEmpty(model.ClientId)
            && string.IsNullOrEmpty(model.SessionId)
            && string.IsNullOrEmpty(model.GrantKey))
        {
            return "ClientId must be used together with SessionId.";
        }
        // GrantKey 已经唯一确定一行，再带 SessionId 语义冲突（过滤会静默以 GrantKey 为准），直接拒绝
        if (!string.IsNullOrEmpty(model.GrantKey) && !string.IsNullOrEmpty(model.SessionId))
        {
            return "GrantKey and SessionId cannot be used together.";
        }
        return null;
    }

    /// <summary>"踢该用户全部会话"：既没有 GrantKey 也没有 SessionId</summary>
    internal static bool IsKickAll(KickOnlineUserModel model)
        => string.IsNullOrEmpty(model.GrantKey) && string.IsNullOrEmpty(model.SessionId);

    /// <summary>
    /// 踢人四级粒度的行过滤：GrantKey &gt; sid+client &gt; sid &gt; 全部（均强制限定 SubjectId）。
    /// **所有分支统一排除 user_consent**：踢人只终止会话，不撤销用户的授权同意。
    /// 其余类型（refresh_token / reference_token / authorization_code / sso_session）一律删除——
    /// 用排除式而非白名单，以后出现新的 grant 类型默认也会被删，对踢人更安全。
    /// </summary>
    internal static IQueryable<PersistedGrant> FilterForKick(IQueryable<PersistedGrant> query, KickOnlineUserModel model)
    {
        var grantsQuery = query.Where(g => g.SubjectId == model.SubjectId
                                        && g.Type != OnlineUserConsts.UserConsentType);
        if (!string.IsNullOrEmpty(model.GrantKey))
        {
            return grantsQuery.Where(g => g.Key == model.GrantKey);
        }
        if (!string.IsNullOrEmpty(model.SessionId))
        {
            return string.IsNullOrEmpty(model.ClientId)
                ? grantsQuery.Where(g => g.SessionId == model.SessionId)
                : grantsQuery.Where(g => g.SessionId == model.SessionId && g.ClientId == model.ClientId);
        }
        if (!string.IsNullOrEmpty(model.ClientId))
        {
            // 防御：即使绕过了 ValidateKick，也不能让"只有 ClientId"退化为踢全部
            return grantsQuery.Where(_ => false);
        }
        return grantsQuery;
    }

    /// <summary>
    /// 需要写 sid 水位线（拒绝该浏览器的门户 Cookie）的会话 id。
    /// - 踢全部：sub 水位线已覆盖该用户全部 Cookie，不需要 sid；
    /// - 踢整个浏览器会话：被删行涉及的 sid；
    /// - 踢单个 App 登录 / sid+client：不拒绝门户 Cookie（粒度只到客户端），除非被删的正是门户行。
    /// </summary>
    internal static List<string> GetWatermarkSessionIds(KickOnlineUserModel model, IEnumerable<PersistedGrant> deleted)
    {
        if (IsKickAll(model))
        {
            return new List<string>();
        }

        var wholeBrowser = !string.IsNullOrEmpty(model.SessionId)
            && string.IsNullOrEmpty(model.ClientId)
            && string.IsNullOrEmpty(model.GrantKey);

        return deleted
            .Where(g => !string.IsNullOrEmpty(g.SessionId)
                && (wholeBrowser || g.Type == OnlineUserConsts.PortalSessionType))
            .Select(g => g.SessionId!)
            .Distinct()
            .ToList();
    }

    /// <summary>
    /// 按被删行生成 backchannel 通知目标：每个 sid 一个目标；无 sid 的 App 登录合为一个 SessionId=null 的目标。
    /// 只有 refresh_token 行代表可通知的客户端会话，门户行（Type=sso_session，ClientId 只是占位值）不参与。
    /// </summary>
    internal static List<KickTarget> BuildKickTargets(IEnumerable<PersistedGrant> deleted)
    {
        return deleted
            .Where(g => g.Type == OnlineUserConsts.RefreshTokenType)
            .GroupBy(g => string.IsNullOrEmpty(g.SessionId) ? null : g.SessionId)
            .Select(g => new KickTarget(g.Key, g.Select(x => x.ClientId).Distinct().ToList()))
            .ToList();
    }

    private static string BuildKickDescription(
        KickOnlineUserModel model, string? operatorName, int removedCount, List<string> clientIds)
    {
        var scope = !string.IsNullOrEmpty(model.GrantKey)
            ? "单个登录会话"
            : !string.IsNullOrEmpty(model.SessionId)
                ? string.IsNullOrEmpty(model.ClientId) ? "浏览器会话" : "浏览器会话中的单个客户端"
                : "全部会话";
        var clients = clientIds.Count > 0 ? $"（{string.Join(",", clientIds)}）" : string.Empty;
        return $"管理员 {operatorName ?? "未知"} 强制下线用户 {model.SubjectId} 的{scope}，移除 {removedCount} 条授权记录{clients}";
    }

    #endregion
}