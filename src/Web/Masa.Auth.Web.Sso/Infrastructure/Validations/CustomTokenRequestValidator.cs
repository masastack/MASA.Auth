// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Web.Sso.Infrastructure.OnlineUser;
using MasaGrantType = Masa.BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType;

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

/// <summary>
/// 自定义 token 请求校验器，IS4 每条 /connect/token 请求只会注入这一个
/// <see cref="ICustomTokenRequestValidator"/>（重复 Add 会顶掉这里的逻辑，扩展请改本类）。
///
/// IS4 的 <c>TokenRequestValidator.RunValidationAsync</c> 先跑 grant 类型校验（password /
/// 扩展 grant 在此期间完成用户验证并赋值 Subject），**之后**才调用本验证器；令牌签发（含
/// refresh_token 行 INSERT）又在本验证器之后。因此这里是"单活 App 会话"清理的安全挂载点：
/// subject 已知、本次新行尚未写入。
///
/// 清理策略：token-only 登录（无浏览器会话，本次签发的 refresh_token 行 SessionId 为 null，
/// 且本次请求申请了 offline_access、确实会签发新的 refresh_token）
/// 成功时，删除同 (SubjectId, ClientId) 的旧 refresh_token 行，保持该用户在该 App 客户端
/// 只有一个有效会话。旧设备下次 refresh 得到 invalid_grant（标准 OIDC 行为）。
/// 仅清理 SessionId == null 的行：浏览器 code 流（Web 渠道）与 sso_session 门户行不受影响。
///
/// 另一件互补的清理：refresh 请求上修剪同一续期链里更早的已消费行（<see cref="PruneConsumedRefreshTokensAsync"/>）。
/// 单活策略只在"新登录"时把上一次登录的整条链清掉；而移动端登录一次后常年只靠 refresh 续期，
/// 这条链上每次轮换都会留下一个已消费旧行，不修剪就随 AT 缩短成倍增长。
/// </summary>
public class CustomTokenRequestValidator : ICustomTokenRequestValidator
{
    readonly PersistedGrantDbContext _persistedGrantDbContext;
    readonly ILogger<CustomTokenRequestValidator> _logger;

    public CustomTokenRequestValidator(
        PersistedGrantDbContext persistedGrantDbContext,
        ILogger<CustomTokenRequestValidator> logger)
    {
        _persistedGrantDbContext = persistedGrantDbContext;
        _logger = logger;
    }

    public async Task ValidateAsync(CustomTokenRequestValidationContext context)
    {
        context.Result.ValidatedRequest.Client.AlwaysSendClientClaims = true;

        // Initialize CustomResponse dictionary if it doesn't exist
        if (context.Result.CustomResponse == null)
        {
            context.Result.CustomResponse = new Dictionary<string, object>();
        }

        // Add api_version to the CustomResponse without overwriting existing values
        context.Result.CustomResponse["api_version"] = "v1.0";

        await RevokeStaleAppSessionsAsync(context.Result.ValidatedRequest);
        await PruneConsumedRefreshTokensAsync(context.Result.ValidatedRequest);
    }

    /// <summary>
    /// refresh 请求上的"修剪"：删除同一续期链（SubjectId + ClientId + SessionId）上更早的已消费 refresh_token 行。
    ///
    /// 为什么挂在这里（反编译 IS4 4.1.2 确认）：<c>ValidateRefreshTokenRequestAsync</c> 已经赋值
    /// <c>ValidatedRequest.RefreshToken</c> / <c>Subject</c>，之后才调用本验证器；而"当前行标记已消费 + 插入新行"
    /// 发生在后面的令牌签发（<c>DefaultRefreshTokenService.UpdateRefreshTokenAsync</c>）里。
    /// 所以此刻当前行还是存活的，库里已消费的全是历史行，全部删掉即可：本次轮换结束后链上只剩
    /// "新存活行 + 刚被消费的当前行"，Admin 在线判定依赖的"最近续期时刻"（已消费行的 ConsumedTime）不丢。
    ///
    /// 链键取自 refresh token 本身（RefreshToken.SubjectId / ClientId / SessionId，即落库行的同名列），
    /// 不取请求上的 SessionId——refresh 请求的 ValidatedRequest.SessionId 恒为空。
    ///
    /// 只对 OneTimeOnly 客户端生效：ReUse 不轮换，没有已消费行，省一次往返。
    /// 安全性：IS4 4.1.2 没有 refresh token 重用检测（<c>AcceptConsumedTokenAsync</c> 默认拒绝，仓库内无自定义
    /// IRefreshTokenService），已消费 token 被重放时，"已消费"和"不存在"的结果都是 invalid_grant。
    /// 失败只记 Warning、不阻断刷新：最坏是旧行多留一轮，下次 refresh 再清；
    /// 若删除后签发失败，该会话的"最近续期"暂时退化为登录时刻，下次成功 refresh 即恢复。
    /// </summary>
    async Task PruneConsumedRefreshTokensAsync(ValidatedTokenRequest request)
    {
        var refreshToken = request.RefreshToken;
        if (request.GrantType != OidcConstants.GrantTypes.RefreshToken
            || refreshToken == null
            || request.Client.RefreshTokenUsage != TokenUsage.OneTimeOnly)
        {
            return;
        }

        var subjectId = refreshToken.SubjectId;
        var clientId = refreshToken.ClientId;
        if (string.IsNullOrEmpty(subjectId) || string.IsNullOrEmpty(clientId))
        {
            return;
        }

        try
        {
            // 用 PersistedGrantDbContext.PersistedGrants（IS4 EF 实体），不要裸写 Set<PersistedGrant>()：
            // 全局命名空间里有 Masa 包的同名实体，会被优先绑定，运行时抛"不在模型里"（见 RefreshTokenGrantFilters）
            var removed = await RefreshTokenGrantFilters
                .ConsumedInChain(_persistedGrantDbContext.PersistedGrants, subjectId, clientId, refreshToken.SessionId)
                .ExecuteDeleteAsync();

            if (removed > 0)
            {
                _logger.LogDebug(
                    "Pruned {Count} consumed refresh_token row(s) of subject {SubjectId} on client {ClientId} (session {SessionId})",
                    removed, subjectId, clientId, refreshToken.SessionId ?? "<none>");
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to prune consumed refresh_token rows of subject {SubjectId} on client {ClientId}",
                subjectId, clientId);
        }
    }

    /// <summary>
    /// 单活 App 会话清理，判定是语义的——"本次签发的 refresh_token 行会不会是 SessionId == null"
    /// （与 Admin 列表侧 <c>ResolveChannel</c> 的 App 渠道定义同一个概念）：
    /// IS4 中 <c>ValidatedRequest.SessionId</c> 只在 authorization_code 分支被赋值（来自浏览器授权时的
    /// code grant），password / 扩展 grant / refresh_token / client_credentials 分支恒为 null（反编译
    /// 4.1.2 确认）；且它是落库行 SessionId 的唯一来源（ValidatedRequest.SessionId → sid claim →
    /// Token/RefreshToken.SessionId → PersistedGrant.SessionId）。在此读它即是对未来那行的精确预测，
    /// 新增扩展授权验证器无需维护任何名单，天然纳入单活语义。
    /// 两个显式排除：refresh_token（轮换续期而非新登录，该请求的 SessionId 同样为空，不排除会
    /// 杀死正在续期的会话）、token-exchange（管理员模拟登录，不能杀被模拟者的真实会话；
    /// 旧的 impersonation 扩展授权已在 #1499 改造为 RFC 8693 token-exchange。TokenExchangeGrantValidator
    /// 另有 offline_access 禁令挡住 refresh_token 签发，此处排除与之互为冗余保险）。
    ///
    /// 另一个必要条件：**本次请求确实会签发 refresh_token**，即请求的 scope 里带了 offline_access
    /// （<see cref="RequestedOfflineAccess"/>）。不能看 <c>Client.AllowOfflineAccess</c>：那只是客户端"被允许"
    /// （新建客户端默认就是 true），一次只申请 <c>openid profile</c> 的登录不会产生新的 refresh_token 行，
    /// 若照样清理，会在没有任何替代会话的情况下杀掉用户在另一台设备上的会话。
    /// 清理失败仅记日志不阻断登录（最坏退化为旧行堆积，下次登录重试）。
    /// </summary>
    async Task RevokeStaleAppSessionsAsync(ValidatedTokenRequest request)
    {
        var clientId = request.Client.ClientId;
        var subjectId = request.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;

        if (request.GrantType == OidcConstants.GrantTypes.RefreshToken
            || request.GrantType == MasaGrantType.TOKEN_EXCHANGE
            || !string.IsNullOrEmpty(request.SessionId)
            || !RequestedOfflineAccess(request)
            || string.IsNullOrEmpty(subjectId))
        {
            return;
        }

        try
        {
            // 必须用 PersistedGrantDbContext.PersistedGrants（IS4 EF 实体）。此前这里写的是 Set<PersistedGrant>()，
            // 会绑到全局命名空间里 Masa 包的同名实体：编译通过，但它不在 SSO 这个 DbContext 的模型里，
            // 运行时抛异常被下面的 catch 吞掉，单活策略等于从未生效（见 RefreshTokenGrantFilters）
            var removed = await RefreshTokenGrantFilters
                .StaleAppSessions(_persistedGrantDbContext.PersistedGrants, subjectId, clientId)
                .ExecuteDeleteAsync();

            if (removed > 0)
            {
                _logger.LogInformation(
                    "Single-active App session policy: revoked {Count} stale refresh_token grant(s) of subject {SubjectId} on client {ClientId} before issuing a new one",
                    removed, subjectId, clientId);
            }
        }
        catch (Exception ex)
        {
            // 删除失败不应阻断登录：旧行最多堆积到自然过期，下次登录会再清理
            _logger.LogWarning(ex,
                "Single-active App session policy: failed to revoke stale refresh_token grants of subject {SubjectId} on client {ClientId}",
                subjectId, clientId);
        }
    }

    /// <summary>
    /// 本次请求是否申请了 offline_access（IS4 据此决定是否签发 refresh_token）。
    /// <c>ValidatedResources.Resources.OfflineAccess</c> 由 scope 校验阶段赋值（先于本验证器），
    /// 为 true 意味着 offline_access 既被请求、又在客户端允许范围内（客户端不允许时 scope 校验直接失败）。
    /// 取不到（为 null）按"未申请"处理：宁可少清理，也不能在不会产生新会话时杀掉旧会话。
    /// </summary>
    static bool RequestedOfflineAccess(ValidatedTokenRequest request)
        => request.ValidatedResources?.Resources?.OfflineAccess == true;
}
