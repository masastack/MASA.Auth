// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.Authentication.Identity;
using Masa.BuildingBlocks.StackSdks.Config;
using TokenProvider = Masa.Contrib.StackSdks.Caller.TokenProvider;

namespace Masa.Auth.Web.Sso.Infrastructure.Oidc;

/// <summary>
/// 模拟登录里"代表 actor 向 Auth 服务发起的调用"。/connect/token 请求上没有已认证的当前用户（actor 的 token 在
/// <c>actor_token</c> 表单参数里），而 SDK 默认只做两件事：从 <see cref="IUserContext"/> 取 userId、把入站请求的
/// <c>Authorization</c> 头转发为 Bearer。所以两类调用各自补上缺的那一块：
/// <list type="bullet">
/// <item><see cref="IsAuthorizedAsync"/>：<c>AuthorizedAsync(appId, code)</c> 的 userId 取自 <see cref="IUserContext"/>，
/// 先用 <see cref="IUserSetter.Change{TIdentityUser}"/> 把当前用户临时切到 actor，调用结束后还原；
/// appId 为 <c>GetServiceId(Auth)</c>（权限由 AppId + Code 唯一确定）；异常 fail-closed。</item>
/// <item><see cref="GetUserAsync"/>：<c>api/user/byId</c> 要求登录身份，不带 Bearer 会 401，
/// 故调用期间把 <see cref="TokenProvider.AccessToken"/> 换成 <c>actor_token</c>，结束后恢复；异常原样抛出，由校验器按拒绝处理。</item>
/// </list>
/// 两者互不相干：Bearer 决定"谁在调用 Auth 服务"，<see cref="IUserContext"/> 决定 SDK 拼进 URL 的 userId。
/// </summary>
public class ActorPermissionClient
{
    readonly IAuthClient _authClient;
    readonly IUserSetter _userSetter;
    readonly IMasaStackConfig _masaStackConfig;
    readonly TokenProvider _tokenProvider;
    readonly ILogger<ActorPermissionClient> _logger;

    public ActorPermissionClient(
        IAuthClient authClient,
        IUserSetter userSetter,
        IMasaStackConfig masaStackConfig,
        TokenProvider tokenProvider,
        ILogger<ActorPermissionClient> logger)
    {
        _authClient = authClient;
        _userSetter = userSetter;
        _masaStackConfig = masaStackConfig;
        _tokenProvider = tokenProvider;
        _logger = logger;
    }

    /// <summary>actor 是否持有指定权限码。</summary>
    public virtual async Task<bool> IsAuthorizedAsync(Guid userId, string permissionCode)
    {
        try
        {
            var appId = _masaStackConfig.GetServiceId(MasaStackProject.Auth);

            // 必须是 IdentityUser 本身：Change 按实际类型存放，GetUserId 读的正是 IdentityUser 这一格。
            // 必须在 using 内 await 完，作用域才能覆盖整个调用。
            using (_userSetter.Change(new IdentityUser { Id = userId.ToString() }))
            {
                return await _authClient.PermissionService.AuthorizedAsync(appId, permissionCode);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Actor permission check failed, denying impersonation (fail-closed). userId:{UserId}, code:{Code}",
                userId, permissionCode);
            return false;
        }
    }

    /// <summary>
    /// 以 actor 的身份查询用户（actor 自己或被模拟者）。<paramref name="actorToken"/> 须是已验签的 <c>actor_token</c>。
    /// 转发只在本次调用期间生效，之后恢复入站请求原有的 token，不影响同一请求里的其它 SDK 调用。
    /// </summary>
    public virtual async Task<UserModel?> GetUserAsync(Guid userId, string actorToken)
    {
        var inboundToken = _tokenProvider.AccessToken;
        _tokenProvider.AccessToken = actorToken;
        try
        {
            return await _authClient.UserService.GetDetailAsync(userId);
        }
        finally
        {
            _tokenProvider.AccessToken = inboundToken;
        }
    }
}
