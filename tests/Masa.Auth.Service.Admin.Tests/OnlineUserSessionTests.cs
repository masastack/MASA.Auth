// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;
using Masa.Auth.Contracts.Admin.Subjects;
using Masa.Auth.Service.Admin.Services;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Entities;
using Masa.BuildingBlocks.Authentication.OpenIdConnect.Domain.Enums;
using Masa.BuildingBlocks.Dispatcher.Events;
using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.Auth.Service.Admin.Application.Subjects.Queries;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using System.Data.Common;
using Xunit;

namespace Masa.Auth.Service.Admin.Tests;

/// <summary>
/// 在线用户会话模型的语义测试（auth.PersistedGrant 为会话事实源）：
/// 1. 会话分组：Web（sid 非空）按 (sid, clientId) 组；App（sid 为空）按 (sub, clientId) 组并并入已消费旧行；
///    sso_session 门户行独立成组并以固定名称 "SSO 门户" 展示；
/// 2. 活跃时间：IS4 轮换**复用同一个 RefreshToken 对象**、落库沿用原 CreationTime，
///    所以同族所有行的 CreationTime 恒等于登录时刻，真正的续期时刻在已消费旧行的 ConsumedTime 上。
///    夹具按这个真实形态构造（同族 CreationTime 相同、仅 ConsumedTime 不同），
///    否则"取 MAX(CreationTime)"这类错误实现会被测成绿的；
/// 3. 在线过滤：Expiration 过期的行不计入；整组都已消费（续期链已死）不展示；
///    最近续期早于阈值的用户 IsOnline=false（onlyOnline=false 时仍出现在列表）；
/// 4. 踢人粒度：单 grantKey / sid+client / sid / 全部（均限定 SubjectId）；
///    所有分支都保留 user_consent；单独的 ClientId 无效（400 / 不匹配任何行）；
/// 5. 踢人的派生结果：水位线 sid 集合（<see cref="OnlineUserService.GetWatermarkSessionIds"/>）、
///    backchannel 通知目标（<see cref="OnlineUserService.BuildKickTargets"/>，按 sid 拆分）。
/// </summary>
public class OnlineUserSessionTests : IAsyncLifetime
{
    readonly SqliteAuthDb _db;

    public OnlineUserSessionTests()
    {
        _db = new SqliteAuthDb();
    }

    static readonly DateTime Now = new(2026, 11, 12, 8, 0, 0, DateTimeKind.Utc);
    static readonly string SubA = "11111111-1111-1111-1111-111111111111";
    static readonly string SubB = "22222222-2222-2222-2222-222222222222";
    static readonly string SubX = "33333333-3333-3333-3333-333333333333";
    static readonly string SubY = "44444444-4444-4444-4444-444444444444";

    async Task SeedAsync()
    {
        var grants = _db.Context.Set<PersistedGrant>();
        var loginAt = Now.AddHours(-2);
        grants.AddRange(
            // sub-A：浏览器 sid-1 登录 webapp 后每小时续期一次，共三代行。
            // IS4 真实形态：三行 CreationTime 全部等于登录时刻，只有 ConsumedTime 不同
            Row("rt-old", "refresh_token", SubA, "webapp", "sid-1", loginAt, Now.AddDays(10), consumed: Now.AddMinutes(-40)),
            Row("rt-a1", "refresh_token", SubA, "webapp", "sid-1", loginAt, Now.AddDays(10), consumed: Now.AddMinutes(-30)),
            Row("rt-a2", "refresh_token", SubA, "webapp", "sid-1", loginAt, Now.AddDays(10)),
            // sub-A：同一浏览器会话下的另一个客户端，从未续期 → 最近续期退化为登录时刻
            Row("rt-a3", "refresh_token", SubA, "webapp2", "sid-1", loginAt, Now.AddDays(10)),
            // sub-A：门户会话行（记住我 30 天），CreationTime 只在登录时写一次
            Row("sso-session:a:sid-1", OnlineUserConsts.PortalSessionType, SubA, "sso-portal", "sid-1", loginAt, Now.AddDays(30)),
            // sub-A：App（ROPC，无 sid）
            Row("rt-a4", "refresh_token", SubA, "app1", null, Now.AddMinutes(-10), Now.AddDays(10)),
            // sub-A：授权同意（用户对 webapp 的 consent）不是会话：不进在线列表，踢人时也必须保留
            Row("consent-a1", OnlineUserConsts.UserConsentType, SubA, "webapp", null, Now.AddDays(-5), Now.AddDays(60)),
            // sub-B：10 小时前登录且从未续期 → 超出默认阈值（120 分钟），IsOnline=false 但有存活行
            Row("rt-b1", "refresh_token", SubB, "app1", null, Now.AddHours(-10), Now.AddDays(10)),
            // sub-B：已过期的行不计入
            Row("rt-b2", "refresh_token", SubB, "app1", null, Now.AddHours(-20), Now.AddHours(-15)));
        _db.Context.Set<Client>().Add(new Client(ClientTypes.Web, "webapp", "Web 应用"));
        _db.Context.Set<Client>().Add(new Client(ClientTypes.Native, "app1", "手机 App"));
        await _db.Context.SaveChangesAsync();
    }

    static PersistedGrant Row(string key, string type, string sub, string client, string? sid, DateTime creation, DateTime expiration, DateTime? consumed = null)
    {
        // Masa 实体属性为 private set，测试里经反射赋值
        var grant = new PersistedGrant();
        Set(grant, nameof(PersistedGrant.Key), key);
        Set(grant, nameof(PersistedGrant.Type), type);
        Set(grant, nameof(PersistedGrant.SubjectId), sub);
        Set(grant, nameof(PersistedGrant.ClientId), client);
        Set(grant, nameof(PersistedGrant.SessionId), sid);
        Set(grant, nameof(PersistedGrant.CreationTime), creation);
        Set(grant, nameof(PersistedGrant.Expiration), expiration);
        Set(grant, nameof(PersistedGrant.ConsumedTime), consumed);
        Set(grant, nameof(PersistedGrant.Data), "{}");
        return grant;
    }

    static void Set<T>(T target, string propertyName, object? value) where T : class =>
        typeof(T).GetProperty(propertyName)!.SetValue(target, value);

    [Fact]
    public async Task Sessions_AreGrouped_WithChannelAndThreshold()
    {
        var users = await BuildAsync();

        var userA = users.Single(u => u.UserId == Guid.Parse(SubA));
        Assert.True(userA.IsOnline);
        Assert.Equal(4, userA.Sessions.Count); // web:webapp / web:webapp2 / web:门户 / app:app1

        var portalSession = userA.Sessions.Single(s => s.ClientName == "SSO 门户");
        Assert.Equal(OnlineUserConsts.ChannelWeb, portalSession.Channel);
        Assert.Equal("sid-1", portalSession.SessionId);
        Assert.Null(portalSession.GrantKey); // 门户/浏览器会话按 sid 踢，无单行 key

        var webApp = userA.Sessions.Single(s => s.ClientId == "webapp");
        Assert.Equal("Web 应用", webApp.ClientName);
        Assert.Equal(ClientTypes.Web, webApp.ClientType);
        Assert.Equal(OnlineUserConsts.ChannelWeb, webApp.Channel);
        // 三代行共享登录时刻：LoginTime 是登录时间，不是"窗口起点"
        Assert.Equal(Now.AddHours(-2), webApp.LoginTime);
        // 最近续期来自已消费旧行的 ConsumedTime（-30min），不是 MAX(CreationTime)（-2h）
        Assert.Equal(Now.AddMinutes(-30), webApp.LastRenewedAt);

        // 从未续期的会话：最近续期退化为 CreationTime（登录时刻）
        var neverRenewed = userA.Sessions.Single(s => s.ClientId == "webapp2");
        Assert.Equal(Now.AddHours(-2), neverRenewed.LoginTime);
        Assert.Equal(Now.AddHours(-2), neverRenewed.LastRenewedAt);

        var app1 = userA.Sessions.Single(s => s.ClientId == "app1" && s.SessionId == null);
        Assert.Equal("手机 App", app1.ClientName);
        Assert.Equal(ClientTypes.Native, app1.ClientType);
        Assert.Equal(OnlineUserConsts.ChannelApp, app1.Channel);
        Assert.Equal("rt-a4", app1.GrantKey); // App 会话按存活行的 Key 踢

        var userB = users.Single(u => u.UserId == Guid.Parse(SubB));
        Assert.False(userB.IsOnline); // 最近续期早于阈值
        Assert.Single(userB.Sessions); // 过期行不计入
        // 排序：最近续期者优先
        Assert.Equal(userA.UserId, users[0].UserId);
    }

    [Fact]
    public async Task LongSession_RenewedHourly_StaysOnline()
    {
        // 回归用例：8 小时前登录、此后每小时轮换一次。
        // 同族所有行 CreationTime 都是登录时刻（IS4 真实形态），只有 ConsumedTime 逐小时推进。
        // 旧实现取 MAX(CreationTime) 会把这种"一直在用"的用户判成离线。
        using var db = new SqliteAuthDb();
        var loginAt = Now.AddHours(-8);
        var grants = db.Context.Set<PersistedGrant>();
        var rows = new List<PersistedGrant>();
        for (var hour = 1; hour <= 7; hour++)
        {
            // 每次轮换：旧行被打上 ConsumedTime，新行未消费
            rows.Add(Row($"rt-x{hour}", "refresh_token", SubX, "app1", null, loginAt, Now.AddDays(10), consumed: loginAt.AddHours(hour)));
        }
        rows.Add(Row("rt-x-alive", "refresh_token", SubX, "app1", null, loginAt, Now.AddDays(10)));
        grants.AddRange(rows);
        await db.Context.SaveChangesAsync();

        var users = await BuildWithAsync(db);
        var user = Assert.Single(users);

        Assert.Equal(Now.AddHours(-1), user.Sessions.Single().LastRenewedAt); // 最后一次续期在 1 小时前
        Assert.Equal(loginAt, user.Sessions.Single().LoginTime);
        Assert.True(user.IsOnline);
    }

    [Fact]
    public async Task FullyConsumedChain_IsNotShown_ButOnlyOnlineToggleDoesNotResurrectIt()
    {
        // 续期链已终结（例如被顶号/踢人后残留）：所有行都已消费，没有可用的 refresh_token。
        // 这些行只是历史活跃记录，不能作为会话展示。
        using var db = new SqliteAuthDb();
        db.Context.Set<PersistedGrant>().AddRange(
            Row("rt-y1", "refresh_token", SubY, "app1", null, Now.AddHours(-3), Now.AddDays(10), consumed: Now.AddHours(-2)),
            Row("rt-y2", "refresh_token", SubY, "app1", null, Now.AddHours(-3), Now.AddDays(10), consumed: Now.AddMinutes(-5)));
        await db.Context.SaveChangesAsync();

        Assert.Empty(await BuildWithAsync(db));
        Assert.Empty(await BuildWithAsync(db, onlyOnline: false));
    }

    [Fact]
    public async Task OnlyOnline_IgnoresRecentActivityOfDeadChains()
    {
        // 边界：SQL 侧的 subject 级聚合会把"已消费行的最近续期"也算进 LastRenewedAt。
        // sub-Y 的 webapp 链 1 分钟前刚被终结（所有行都已消费），只剩 10 小时前的门户行——
        // 存活会话的最近续期是 10 小时前，必须被判为离线并从默认列表里剔除。
        using var db = new SqliteAuthDb();
        db.Context.Set<PersistedGrant>().AddRange(
            Row("rt-y-dead", "refresh_token", SubY, "webapp", "sid-y", Now.AddHours(-10), Now.AddDays(10), consumed: Now.AddMinutes(-1)),
            Row("sso-session:y:sid-y", OnlineUserConsts.PortalSessionType, SubY, "sso-portal", "sid-y", Now.AddHours(-10), Now.AddDays(30)));
        await db.Context.SaveChangesAsync();

        Assert.Empty(await BuildWithAsync(db, onlyOnline: true));

        var all = Assert.Single(await BuildWithAsync(db, onlyOnline: false));
        Assert.False(all.IsOnline);
        // 只剩门户会话（webapp 链已终态，不展示）
        Assert.Equal("sso-portal", all.Sessions.Single().ClientId);
    }

    [Fact]
    public async Task OnlyOnline_FiltersIdleUsers_ByDefault()
    {
        // sub-B 最近续期是 10 小时前，超出默认阈值：默认列表只返回在线用户
        var online = await BuildAsync(onlyOnline: true);
        Assert.DoesNotContain(online, u => u.UserId == Guid.Parse(SubB));
        Assert.Contains(online, u => u.UserId == Guid.Parse(SubA));

        // 关掉开关用于排查"谁还挂着会话"：闲置但仍有存活行的用户回来了
        var all = await BuildAsync(onlyOnline: false);
        var userB = all.Single(u => u.UserId == Guid.Parse(SubB));
        Assert.False(userB.IsOnline);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task CandidateSelection_IsPushedDownAsDistinctSubjects_NotAFullTableGroupBy(bool onlyOnline)
    {
        // 候选用户只取 DISTINCT SubjectId，不再对整张 PersistedGrant 做 GROUP BY 聚合回传"每个有 grant 的用户一行"。
        // 这个接口常被前端轮询，闲置 grant（默认保留 30 天）远多于在线会话，聚合会让开销随闲置量增长而不是随在线量增长
        var commands = new CommandCapture();
        using var db = new SqliteAuthDb(commands);
        var idle = Enumerable.Range(0, 30).Select(i =>
            Row($"rt-idle-{i}", "refresh_token", Guid.NewGuid().ToString(), "app1", null, Now.AddHours(-10), Now.AddDays(10)));
        db.Context.Set<PersistedGrant>().AddRange(idle.Append(
            Row("rt-online", "refresh_token", SubA, "app1", null, Now.AddMinutes(-5), Now.AddDays(10))));
        await db.Context.SaveChangesAsync();
        commands.Clear();

        var users = await BuildWithAsync(db, onlyOnline: onlyOnline);

        Assert.All(commands.Statements, sql => Assert.DoesNotContain("GROUP BY", sql, StringComparison.OrdinalIgnoreCase));
        Assert.Contains(commands.Statements, sql => sql.Contains("DISTINCT", StringComparison.OrdinalIgnoreCase));
        // onlyOnline：只有近期活跃的用户；否则：所有有存活会话的用户（含 30 个闲置的）
        Assert.Equal(onlyOnline ? 1 : 31, users.Count);
        Assert.Equal(Guid.Parse(SubA), users[0].UserId); // 最近续期者排第一
    }

    [Fact]
    public async Task Results_AreOrderedByLiveSessionLastRenewal_WithStableTieBreak()
    {
        // 候选集来自 DISTINCT 查询、没有固有顺序，排序必须由内存里的"存活会话最近续期"决定，并且稳定
        using var db = new SqliteAuthDb();
        db.Context.Set<PersistedGrant>().AddRange(
            Row("rt-1", "refresh_token", SubB, "app1", null, Now.AddMinutes(-50), Now.AddDays(10)),
            Row("rt-2", "refresh_token", SubX, "app1", null, Now.AddMinutes(-5), Now.AddDays(10)),
            Row("rt-3", "refresh_token", SubA, "app1", null, Now.AddMinutes(-20), Now.AddDays(10)),
            Row("rt-4", "refresh_token", SubY, "app1", null, Now.AddMinutes(-20), Now.AddDays(10))); // 与 SubA 并列
        await db.Context.SaveChangesAsync();

        var users = await BuildWithAsync(db, onlyOnline: true);

        Assert.Equal(
            new[] { SubX, SubA, SubY, SubB }.Select(Guid.Parse).ToArray(),
            users.Select(u => u.UserId).ToArray());
    }

    [Fact]
    public async Task NonGuidSubject_IsIgnored_InsteadOfFailingTheWholeList()
    {
        // grant 表里的 SubjectId 是自由字符串。此前候选用户在复核之前就 Guid.Parse，遇到非 GUID 的 subject 整个列表 500
        using var db = new SqliteAuthDb();
        db.Context.Set<PersistedGrant>().AddRange(
            Row("rt-legacy", "refresh_token", "not-a-guid", "app1", null, Now.AddMinutes(-5), Now.AddDays(10)),
            Row("rt-a", "refresh_token", SubA, "app1", null, Now.AddMinutes(-10), Now.AddDays(10)));
        await db.Context.SaveChangesAsync();

        var users = await BuildWithAsync(db, onlyOnline: true);

        Assert.Equal(Guid.Parse(SubA), Assert.Single(users).UserId);
    }

    sealed class CommandCapture : DbCommandInterceptor
    {
        readonly List<string> _statements = new();

        public IReadOnlyList<string> Statements => _statements;

        public void Clear() => _statements.Clear();

        public override InterceptionResult<DbDataReader> ReaderExecuting(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result)
        {
            _statements.Add(command.CommandText);
            return result;
        }

        public override ValueTask<InterceptionResult<DbDataReader>> ReaderExecutingAsync(
            DbCommand command, CommandEventData eventData, InterceptionResult<DbDataReader> result,
            CancellationToken cancellationToken = default)
        {
            _statements.Add(command.CommandText);
            return new(result);
        }
    }

    [Fact]
    public async Task Threshold_ControlsOnlineFlag()
    {
        var users = await BuildAsync(onlineThreshold: TimeSpan.FromHours(11));
        Assert.True(users.Single(u => u.UserId == Guid.Parse(SubB)).IsOnline);

        users = await BuildAsync(onlineThreshold: TimeSpan.FromMinutes(30));
        Assert.False(users.Single(u => u.UserId == Guid.Parse(SubB)).IsOnline);
        // sub-A 最近续期（App 行 -10min）仍在 30 分钟阈值内
        Assert.True(users.Single(u => u.UserId == Guid.Parse(SubA)).IsOnline);
    }

    [Fact]
    public async Task ChannelFilter_App_ReturnsOnlyAppSessions_AndRecomputesCountAndOnline()
    {
        var users = await BuildAsync(channel: OnlineUserConsts.ChannelApp);

        // sub-A 同时有 Web 与 App 会话：只保留 App 的一条；sub-B 只有 App 会话
        var userA = users.Single(u => u.UserId == Guid.Parse(SubA));
        var app = Assert.Single(userA.Sessions);
        Assert.Equal("app1", app.ClientId);
        Assert.Equal(OnlineUserConsts.ChannelApp, app.Channel);
        Assert.Equal(1, userA.SessionCount);
        Assert.True(userA.IsOnline);
        Assert.Contains(users, u => u.UserId == Guid.Parse(SubB));
        Assert.All(users, u => Assert.All(u.Sessions, s => Assert.Equal(OnlineUserConsts.ChannelApp, s.Channel)));
    }

    [Fact]
    public async Task ChannelFilter_Web_ReturnsOnlyWebSessions_AndDropsUsersWithoutThem()
    {
        var users = await BuildAsync(channel: OnlineUserConsts.ChannelWeb);

        // sub-B 只有 App 会话 → 不出现；sub-A 只剩 webapp / webapp2 / 门户 三个 Web 会话
        var userA = Assert.Single(users);
        Assert.Equal(Guid.Parse(SubA), userA.UserId);
        Assert.Equal(3, userA.Sessions.Count);
        Assert.Equal(3, userA.SessionCount);
        Assert.All(userA.Sessions, s => Assert.Equal(OnlineUserConsts.ChannelWeb, s.Channel));
    }

    [Fact]
    public async Task ChannelFilter_IsOnline_IsComputedWithinTheChannel()
    {
        // 阈值 15 分钟：sub-A 的 App 会话（从未续期，最近续期=登录时刻 -10min）在线；
        // Web 侧最近续期是 webapp 的 -30min（来自已消费旧行），已超出
        var app = await BuildAsync(onlineThreshold: TimeSpan.FromMinutes(15), channel: OnlineUserConsts.ChannelApp);
        Assert.True(app.Single(u => u.UserId == Guid.Parse(SubA)).IsOnline);

        var web = await BuildAsync(onlineThreshold: TimeSpan.FromMinutes(15), channel: OnlineUserConsts.ChannelWeb);
        Assert.False(web.Single(u => u.UserId == Guid.Parse(SubA)).IsOnline);
    }

    [Fact]
    public async Task KickFilter_GrantKey_DeletesSingleAppSession()
    {
        var query = _db.Context.Set<PersistedGrant>();
        var removed = await OnlineUserService.FilterForKick(query, new KickOnlineUserModel(SubA, GrantKey: "rt-a4")).ToListAsync();
        var grant = Assert.Single(removed);
        Assert.Equal("rt-a4", grant.Key);
    }

    [Fact]
    public async Task KickFilter_SessionIdPlusClient_DeletesOnlyThatClient()
    {
        var query = _db.Context.Set<PersistedGrant>();
        var removed = await OnlineUserService.FilterForKick(query, new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "webapp")).ToListAsync();
        Assert.Equal(3, removed.Count); // rt-a1 + rt-a2 + 被轮换淘汰的 rt-old（同族整链删除），不含 webapp2 / 门户行
        Assert.All(removed, g => Assert.Equal("webapp", g.ClientId));
    }

    [Fact]
    public async Task KickFilter_SessionIdOnly_DeletesWholeBrowserSession()
    {
        var query = _db.Context.Set<PersistedGrant>();
        var removed = await OnlineUserService.FilterForKick(query, new KickOnlineUserModel(SubA, SessionId: "sid-1")).ToListAsync();
        // webapp 两代 + 淘汰行 + webapp2 + 门户行；不含 App 行
        Assert.Equal(5, removed.Count);
        Assert.DoesNotContain(removed, g => g.SessionId == null);
    }

    [Fact]
    public async Task KickFilter_KickAll_DeletesEverySessionRow_ButKeepsUserConsent()
    {
        var removed = await Kick(new KickOnlineUserModel(SubA));
        // rt-a1/a2/a3/a4 + 淘汰行 rt-old + 门户行；user_consent 保留
        Assert.Equal(6, removed.Count);
        Assert.DoesNotContain(removed, g => g.Type == OnlineUserConsts.UserConsentType);
        Assert.Contains(removed, g => g.Type == OnlineUserConsts.PortalSessionType);
    }

    [Fact]
    public async Task KickFilter_NeverCrossesSubject()
    {
        var removed = await Kick(new KickOnlineUserModel(SubB, SessionId: "sid-1"));
        Assert.Empty(removed); // sid-1 属于 sub-A
    }

    [Fact]
    public async Task KickFilter_ClientIdOnly_MatchesNothing_InsteadOfDegradingToKickAll()
    {
        var removed = await Kick(new KickOnlineUserModel(SubA, ClientId: "webapp"));
        Assert.Empty(removed);
    }

    [Fact]
    public async Task KickAll_ExecuteDelete_RemovesSessionRows_KeepsConsentAndOtherSubjects()
    {
        // 与 KickUserAsync 相同的删除方式：按同一过滤条件 ExecuteDelete（而非按快照主键删）
        var deleted = await OnlineUserService.FilterForKick(
            _db.Context.Set<PersistedGrant>(), new KickOnlineUserModel(SubA)).ExecuteDeleteAsync();
        Assert.Equal(6, deleted);

        var remaining = await _db.Context.Set<PersistedGrant>().AsNoTracking().Select(g => g.Key).ToListAsync();
        Assert.Equal(
            new[] { "consent-a1", "rt-b1", "rt-b2" },
            remaining.OrderBy(k => k, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public async Task KickFilter_AlsoDeletesReferenceTokenRows()
    {
        // reference_token（AccessTokenType=Reference 客户端签发的 opaque AT）行随踢人一并删除：
        // 资源端走 introspection，下一次校验即 inactive——"踢人即时 401"（L3，见 plan 文档 §7/§8）的依据就是这行删除。
        // FilterForKick 是排除式过滤（只排除 user_consent），新 grant 类型默认被删，对踢人更安全
        _db.Context.Set<PersistedGrant>().Add(
            Row("ref-a1", "reference_token", SubA, "secureapp", "sid-1", Now.AddMinutes(-5), Now.AddMinutes(55)));
        await _db.Context.SaveChangesAsync();

        var wholeSession = await Kick(new KickOnlineUserModel(SubA, SessionId: "sid-1"));
        Assert.Contains(wholeSession, g => g.Key == "ref-a1" && g.Type == "reference_token");

        var kickAll = await Kick(new KickOnlineUserModel(SubA));
        Assert.Contains(kickAll, g => g.Key == "ref-a1");

        // 踢 sid+client：reference 行属于该客户端会话，同样被删
        var singleClient = await Kick(new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "secureapp"));
        Assert.Contains(singleClient, g => g.Key == "ref-a1" && g.Type == "reference_token");
    }

    [Fact]
    public void SuggestedOnlineThreshold_DefaultsToTwiceTheDefaultAccessTokenLifetime()
    {
        // 2 × IS4 默认 AT 3600s = 120 分钟，与 OnlineUserConsts.DefaultOnlineThresholdMinutes 对齐（联动公式锚定）
        Assert.Equal(OnlineUserConsts.DefaultOnlineThresholdMinutes, OnlineUserService.SuggestedOnlineThresholdMinutes(3600));
    }

    [Fact]
    public void SuggestedOnlineThreshold_RoundsUp_AndHandlesZero()
    {
        Assert.Equal(30, OnlineUserService.SuggestedOnlineThresholdMinutes(900)); // 15min AT → 30min
        Assert.Equal(21, OnlineUserService.SuggestedOnlineThresholdMinutes(601));   // ceil(1202s / 60)
        Assert.Equal(0, OnlineUserService.SuggestedOnlineThresholdMinutes(0));      // 无启用客户端 → 不约束
    }

    [Fact]
    public void ThresholdTooSmall_DetectsClientLifetimeDrift()
    {
        // 边界：等于建议值视为足够（阈值 == 2 × AT 时不会误判在线用户）
        Assert.False(OnlineUserService.ThresholdTooSmall(120, 3600));
        Assert.True(OnlineUserService.ThresholdTooSmall(119, 3600));
        // 客户端把 AT 调大到 2h、阈值没跟上——运行时告警要捕捉的漂移
        Assert.True(OnlineUserService.ThresholdTooSmall(120, 7200));
        // L1 把 App AT 调短（15min）后，默认阈值 120 依然满足联动
        Assert.False(OnlineUserService.ThresholdTooSmall(120, 900));
    }

    [Fact]
    public void ValidateKick_SubjectIdIsRequired()
    {
        Assert.NotNull(OnlineUserService.ValidateKick(new KickOnlineUserModel(" ")));
        Assert.NotNull(OnlineUserService.ValidateKick(new KickOnlineUserModel(string.Empty, SessionId: "sid-1")));
    }

    [Fact]
    public void ValidateKick_ClientIdWithoutSessionId_IsRejected()
    {
        Assert.NotNull(OnlineUserService.ValidateKick(new KickOnlineUserModel(SubA, ClientId: "webapp")));
    }

    [Fact]
    public void ValidateKick_GrantKeyWithSessionId_IsRejected()
    {
        // GrantKey 已唯一确定一行，再带 SessionId 语义冲突（过滤会静默以 GrantKey 为准）
        Assert.NotNull(OnlineUserService.ValidateKick(
            new KickOnlineUserModel(SubA, SessionId: "sid-1", GrantKey: "rt-a2")));
        // GrantKey + ClientId（无 SessionId）仍合法：GrantKey 优先
        Assert.Null(OnlineUserService.ValidateKick(
            new KickOnlineUserModel(SubA, ClientId: "webapp", GrantKey: "rt-a2")));
    }

    [Fact]
    public async Task KickFilter_EveryBranch_KeepsUserConsent()
    {
        // "踢人不撤销授权同意"是承诺给用户的语义：不只"踢全部"分支要遵守。
        // 这里用指向 consent 行本身的 GrantKey 验证排除条件在所有分支上都生效。
        var byGrantKey = await Kick(new KickOnlineUserModel(SubA, GrantKey: "consent-a1"));
        Assert.Empty(byGrantKey);

        Assert.DoesNotContain(await Kick(new KickOnlineUserModel(SubA)), g => g.Type == OnlineUserConsts.UserConsentType);
        Assert.DoesNotContain(await Kick(new KickOnlineUserModel(SubA, SessionId: "sid-1")),
            g => g.Type == OnlineUserConsts.UserConsentType);
        Assert.DoesNotContain(await Kick(new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "webapp")),
            g => g.Type == OnlineUserConsts.UserConsentType);

        // 踢完再确认 consent 行仍在库里
        await OnlineUserService.FilterForKick(_db.Context.Set<PersistedGrant>(), new KickOnlineUserModel(SubA))
            .ExecuteDeleteAsync();
        var remaining = await _db.Context.Set<PersistedGrant>().AsNoTracking().Select(g => g.Type).ToListAsync();
        Assert.Contains(OnlineUserConsts.UserConsentType, remaining);
    }

    [Fact]
    public void ValidateKick_LegalCombinations_Pass()
    {
        Assert.Null(OnlineUserService.ValidateKick(new KickOnlineUserModel(SubA)));
        Assert.Null(OnlineUserService.ValidateKick(new KickOnlineUserModel(SubA, SessionId: "sid-1")));
        Assert.Null(OnlineUserService.ValidateKick(new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "webapp")));
        Assert.Null(OnlineUserService.ValidateKick(new KickOnlineUserModel(SubA, GrantKey: "rt-a4")));
    }

    [Fact]
    public async Task WatermarkSessionIds_KickAll_IsEmpty_BecauseSubjectWatermarkCoversIt()
    {
        var model = new KickOnlineUserModel(SubA);
        Assert.True(OnlineUserService.IsKickAll(model));
        Assert.Empty(OnlineUserService.GetWatermarkSessionIds(model, await Kick(model)));
    }

    [Fact]
    public async Task WatermarkSessionIds_WholeBrowserSession_ContainsThatSid()
    {
        var model = new KickOnlineUserModel(SubA, SessionId: "sid-1");
        Assert.False(OnlineUserService.IsKickAll(model));
        var sids = OnlineUserService.GetWatermarkSessionIds(model, await Kick(model));
        Assert.Equal(new[] { "sid-1" }, sids);
    }

    [Fact]
    public async Task WatermarkSessionIds_SingleClientOfBrowser_DoesNotRejectPortalCookie()
    {
        var model = new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "webapp");
        Assert.Empty(OnlineUserService.GetWatermarkSessionIds(model, await Kick(model)));
    }

    [Fact]
    public async Task WatermarkSessionIds_SingleAppOrClientGrant_IsEmpty()
    {
        var app = new KickOnlineUserModel(SubA, GrantKey: "rt-a4");
        Assert.Empty(OnlineUserService.GetWatermarkSessionIds(app, await Kick(app)));

        // 删除的是带 sid 的 refresh_token 行（不是门户行）：只终止该客户端，不拒绝门户 Cookie
        var webGrant = new KickOnlineUserModel(SubA, GrantKey: "rt-a2");
        Assert.Empty(OnlineUserService.GetWatermarkSessionIds(webGrant, await Kick(webGrant)));
    }

    [Fact]
    public async Task WatermarkSessionIds_DeletingPortalRowByKey_RejectsThatBrowser()
    {
        var model = new KickOnlineUserModel(SubA, GrantKey: "sso-session:a:sid-1");
        var sids = OnlineUserService.GetWatermarkSessionIds(model, await Kick(model));
        Assert.Equal(new[] { "sid-1" }, sids);
    }

    [Fact]
    public async Task BuildKickTargets_KickAll_SplitsBySid_AndGroupsSidlessAppRowsTogether()
    {
        var targets = OnlineUserService.BuildKickTargets(await Kick(new KickOnlineUserModel(SubA)));

        Assert.Equal(2, targets.Count);
        var web = targets.Single(t => t.SessionId == "sid-1");
        Assert.Equal(new[] { "webapp", "webapp2" }, web.ClientIds.OrderBy(c => c, StringComparer.Ordinal).ToArray());
        var app = targets.Single(t => t.SessionId == null);
        Assert.Equal(new[] { "app1" }, app.ClientIds);
    }

    [Fact]
    public async Task BuildKickTargets_SessionPlusClient_IsSingleTargetWithThatClient()
    {
        var targets = OnlineUserService.BuildKickTargets(
            await Kick(new KickOnlineUserModel(SubA, SessionId: "sid-1", ClientId: "webapp")));

        var target = Assert.Single(targets);
        Assert.Equal("sid-1", target.SessionId);
        Assert.Equal(new[] { "webapp" }, target.ClientIds);
    }

    [Fact]
    public async Task BuildKickTargets_PortalRowOnly_ProducesNoTarget()
    {
        // 门户行的 ClientId 只是占位值，不是真实客户端，不能发 backchannel
        var targets = OnlineUserService.BuildKickTargets(
            await Kick(new KickOnlineUserModel(SubA, GrantKey: "sso-session:a:sid-1")));
        Assert.Empty(targets);
    }

    async Task<List<PersistedGrant>> Kick(KickOnlineUserModel model)
        => await OnlineUserService.FilterForKick(_db.Context.Set<PersistedGrant>().AsNoTracking(), model).ToListAsync();

    async Task<List<OnlineUserDto>> BuildAsync(
        TimeSpan? onlineThreshold = null, string? channel = null, bool onlyOnline = false)
        => await BuildWithAsync(_db, onlineThreshold, channel, onlyOnline);

    /// <summary>默认 onlyOnline=false：多数用例要检查"闲置但仍有存活行"的用户，需要他们出现在结果里</summary>
    async Task<List<OnlineUserDto>> BuildWithAsync(
        SqliteAuthDb db, TimeSpan? onlineThreshold = null, string? channel = null, bool onlyOnline = false)
    {
        var eventBus = StubProxy.Create<IEventBus>((method, args) =>
        {
            if (method.Name == "PublishAsync" && args?[0] is UserPortraitsQuery portraits)
            {
                portraits.Result = new List<UserModel>
                {
                    new() { Id = Guid.Parse(SubA), Account = "alice", DisplayName = "Alice", Avatar = "https://a/avatar.png" },
                    new() { Id = Guid.Parse(SubB), Account = "bob", DisplayName = "Bob", Avatar = "" }
                };
                return (true, Task.CompletedTask);
            }
            return (false, null);
        });

        return await OnlineUserService.BuildOnlineUsersAsync(
            db.Context, db.Context, eventBus, Now,
            onlineThreshold ?? TimeSpan.FromMinutes(OnlineUserConsts.DefaultOnlineThresholdMinutes),
            channelFilter: channel,
            onlyOnline: onlyOnline);
    }

    public Task InitializeAsync() => SeedAsync();
    public Task DisposeAsync()
    {
        _db.Dispose();
        return Task.CompletedTask;
    }
}
