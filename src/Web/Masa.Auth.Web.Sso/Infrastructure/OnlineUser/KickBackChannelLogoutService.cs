// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityModel;
using IdentityServer4;
using IdentityServer4.Configuration;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Masa.BuildingBlocks.StackSdks.Config;
using Microsoft.AspNetCore.Authentication;

namespace Masa.Auth.Web.Sso.Infrastructure.OnlineUser;

/// <summary>
/// 可在后台服务（无 HttpContext）里使用的 OIDC back-channel logout 实现。
///
/// 为什么需要：IS4 4.1.2 的 DefaultBackChannelLogoutService.CreateTokenAsync 调用
/// IdentityServerTools.IssueJwtAsync(lifetime, claims)，后者用
/// ContextAccessor.HttpContext.GetIdentityServerIssuerUri() 取 iss；HttpContext 为空时抛
/// ArgumentNullException。踢人通知由 <see cref="BackgroundJobs.KickRequestConsumer"/> 在后台发送，
/// 没有 HttpContext，所以必须显式传 issuer（IssueJwtAsync 的三参重载不依赖 HttpContext）。
///
/// 行为：
/// - 有 HttpContext（用户主动登出等请求内场景）：完全沿用 IS4 默认行为，issuer 由当前请求推导；
/// - 无 HttpContext：issuer 取 IdentityServerOptions.IssuerUri，未配置则取 SSO 对外地址
///   （与 Admin 校验令牌的 Authority 同源，即 id_token 中的 iss），去掉末尾 '/'；
/// - 客户端要求 sid（BackChannelLogoutSessionRequired，IS4 默认开启）而本次通知没有 sid
///   （无浏览器会话的 App 登录）：IS4 会抛 ArgumentException("Client requires SessionId")，
///   这里改为记录告警并跳过该客户端，不让一个客户端的协议要求拖垮整条通知。
/// </summary>
public class KickBackChannelLogoutService : DefaultBackChannelLogoutService
{
    readonly IHttpContextAccessor _httpContextAccessor;
    readonly IdentityServerOptions _options;
    readonly IMasaStackConfig _masaStackConfig;
    readonly ILogger<IBackChannelLogoutService> _logger;

#pragma warning disable CS0618 // ISystemClock 在 ASP.NET Core 8+ 标记过时，但 IS4 4.1.2 的基类构造函数仍要求它
    public KickBackChannelLogoutService(
        ISystemClock clock,
        IdentityServerTools tools,
        ILogoutNotificationService logoutNotificationService,
        IBackChannelLogoutHttpClient backChannelLogoutHttpClient,
        ILogger<IBackChannelLogoutService> logger,
        IHttpContextAccessor httpContextAccessor,
        IdentityServerOptions options,
        IMasaStackConfig masaStackConfig)
        : base(clock, tools, logoutNotificationService, backChannelLogoutHttpClient, logger)
#pragma warning restore CS0618
    {
        _httpContextAccessor = httpContextAccessor;
        _options = options;
        _masaStackConfig = masaStackConfig;
        _logger = logger;
    }

    protected override async Task<string> CreateTokenAsync(BackChannelLogoutRequest request)
    {
        if (_httpContextAccessor.HttpContext != null)
        {
            return await base.CreateTokenAsync(request);
        }

        var claims = await CreateClaimsForTokenAsync(request);
        if (claims.Any(x => x.Type == JwtClaimTypes.Nonce))
        {
            throw new InvalidOperationException("nonce claim is not allowed in the back-channel signout token.");
        }

        return await Tools.IssueJwtAsync(DefaultLogoutTokenLifetime, ResolveIssuer(), claims);
    }

    protected override Task SendLogoutNotificationAsync(BackChannelLogoutRequest request)
    {
        if (request.SessionIdRequired && string.IsNullOrEmpty(request.SessionId))
        {
            _logger.LogWarning(
                "Skip back-channel logout for client {ClientId}: it requires a session id (BackChannelLogoutSessionRequired) " +
                "but this notification has none (token-only login)",
                request.ClientId);
            return Task.CompletedTask;
        }

        return base.SendLogoutNotificationAsync(request);
    }

    string ResolveIssuer()
    {
        var issuer = _options.IssuerUri;
        if (string.IsNullOrWhiteSpace(issuer))
        {
            issuer = _masaStackConfig.GetSsoDomain();
        }

        if (string.IsNullOrWhiteSpace(issuer))
        {
            throw new InvalidOperationException(
                "Cannot resolve the issuer for back-channel logout outside an HTTP request: " +
                "neither IdentityServerOptions.IssuerUri nor the SSO domain of MasaStackConfig is configured.");
        }

        return issuer.TrimEnd('/');
    }
}
