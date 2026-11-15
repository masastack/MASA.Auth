// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;

namespace Masa.Auth.Web.Sso.Infrastructure.Events;

/// <summary>
/// 门户会话落库：登录成功时在 auth.PersistedGrant 写一行 Type="sso_session"（Key 前缀
/// sso-session:，见 OnlineUserConsts），作为"直登门户 / 未开
/// offline_access 客户端"用户在 Admin 在线列表的可见性来源。
/// IS4 的 StoreAsync 天然按 Key upsert，重复触发（如同 sid 重认证）不会主键冲突。
/// ROPC 等 token-only 登录没有浏览器会话（sid 为空），不写门户行——它们的在线状态
/// 由 refresh_token 行承载。
/// 门户行的删除：主动登出在 AccountController.Logout 里按 sid 精确删除；踢人由 Admin 直接删行；
/// 其他登出路径（如 front-channel）留下的行由 Expiration + TokenCleanup 兜底清理。
/// </summary>
public class IdentityServerEventSink : IEventSink
{
    // PersistedGrant.ClientId 列非空，而门户行不对应任何真实客户端，所以填占位值；
    // Admin 侧靠 Type（sso_session）识别门户行，不依赖这个值
    const string PortalClientId = "sso-portal";

    readonly ILogger<IdentityServerEventSink> _logger;
    readonly IUserSession _userSession;
    readonly IPersistedGrantStore _persistedGrantStore;

    public IdentityServerEventSink(
        ILogger<IdentityServerEventSink> logger,
        IUserSession userSession,
        IPersistedGrantStore persistedGrantStore)
    {
        _logger = logger;
        _userSession = userSession;
        _persistedGrantStore = persistedGrantStore;
    }

    public async Task PersistAsync(Event evt)
    {
        if (evt.EventType == EventTypes.Success ||
            evt.EventType == EventTypes.Information)
        {
            _logger.LogInformation("{Name} ({Id}), Details: {@details}",
                evt.Name,
                evt.Id,
                evt);

            if (evt.Id == EventIds.UserLoginSuccess && evt is UserLoginSuccessEvent login)
            {
                try
                {
                    var sessionId = await _userSession.GetSessionIdAsync();
                    if (!string.IsNullOrEmpty(sessionId))
                    {
                        var now = DateTime.UtcNow;
                        await _persistedGrantStore.StoreAsync(new IdentityServer4.Models.PersistedGrant
                        {
                            Key = $"{OnlineUserConsts.PortalSessionKeyPrefix}{login.SubjectId}:{sessionId}",
                            Type = OnlineUserConsts.PortalSessionType,
                            SubjectId = login.SubjectId,
                            ClientId = PortalClientId,
                            SessionId = sessionId,
                            CreationTime = now,
                            // 服务器无法感知浏览器会话 Cookie 的实际生命周期（记住我 30 天 /
                            // 会话 Cookie 随浏览器关闭失效），统一按最长寿命给 TTL；
                            // 在线展示由 Admin 侧的活跃阈值过滤保证
                            Expiration = now.AddDays(OnlineUserConsts.PortalSessionLifetimeDays),
                            // Data 列非空；门户行没有需要持久化的负载
                            Data = "{}"
                        });
                    }
                }
                catch (Exception ex)
                {
                    // 门户行写入失败只影响列表可见性，不阻断登录流程
                    _logger.LogError(ex, "Failed to store sso_session row for subject {SubjectId}", login.SubjectId);
                }
            }
        }
        else
        {
            _logger.LogError("{Name} ({Id}), Details: {@details}",
                evt.Name,
                evt.Id,
                evt);
        }
    }
}
