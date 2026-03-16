// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Controllers;

[Microsoft.AspNetCore.Mvc.Route("[action]")]
[AllowAnonymous]
public class AccountController : Controller
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly I18n _i18n;
    readonly IBackChannelLogoutService _backChannelClient;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IPersistedGrantService _persistedGrantService;
    readonly ILogger<AccountController> _logger;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IEventService events,
        IAuthClient authClient,
        I18n i18n,
        IBackChannelLogoutService backChannelClient,
        IDistributedCacheClient distributedCacheClient,
        IPersistedGrantService persistedGrantService,
        ILogger<AccountController> logger)
    {
        _interaction = interaction;
        _events = events;
        _authClient = authClient;
        _i18n = i18n;
        _backChannelClient = backChannelClient;
        _distributedCacheClient = distributedCacheClient;
        _persistedGrantService = persistedGrantService;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInputModel inputModel)
    {
        var returnUrl = inputModel.ReturnUrl;
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        try
        {
            UserModel? user = null;

            if (inputModel.PhoneLogin)
            {
                user = await _authClient.UserService.LoginByPhoneNumberAsync(new LoginByPhoneNumberModel
                {
                    PhoneNumber = inputModel.PhoneNumber,
                    Code = inputModel.SmsCode?.ToString() ?? throw new UserFriendlyException(_i18n.T("SmsRequired") ?? "SmsRequired"),
                    RegisterLogin = inputModel.RegisterLogin,
                    Environment = inputModel.Environment
                });
            }
            else
            {
                user = await _authClient.UserService.ValidateAccountAsync(new ValidateAccountModel
                {
                    Account = inputModel.Account,
                    Password = inputModel.Password,
                    LdapLogin = inputModel.LdapLogin,
                    Environment = inputModel.Environment
                });
            }

            if (user != null)
            {
                // only set explicit expiration here if user chooses "remember me". 
                // otherwise we rely upon expiration configured in cookie middleware.
                AuthenticationProperties? props = null;
                if (LoginOptions.AllowRememberLogin && inputModel.RememberLogin)
                {
                    props = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                    };
                }

                var isuser = new IdentityServerUser(user!.Id.ToString())
                {
                    DisplayName = user.DisplayName
                };
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.USER_NAME, user.DisplayName));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.ACCOUNT, user.Account));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.ENVIRONMENT, inputModel.Environment));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.ROLES, JsonSerializer.Serialize(user.Roles.Select(r => r.Code))));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.CURRENT_TEAM, (user.CurrentTeamId ?? Guid.Empty).ToString()));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.STAFF, (user.StaffId ?? Guid.Empty).ToString()));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.PHONE_NUMBER, user.PhoneNumber ?? ""));
                isuser.AdditionalClaims.Add(new Claim(IdentityClaimConsts.EMAIL, user.Email ?? ""));

                //us sign in
                await HttpContext.SignInAsync(isuser, props);

                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Name, user.Id.ToString(), user.DisplayName, clientId: context?.Client.ClientId));

                // Record login operation log (including client information)
                var loginDescription = inputModel.PhoneLogin
                    ? $"用户登录：使用手机号{inputModel.PhoneNumber}登录"
                    : $"用户登录：使用账号{inputModel.Account}登录";
                await RecordLoginOperationLogAsync(user, loginDescription, context?.Client.ClientId);

                if (context != null && context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.
                    return this.LoadingPage(returnUrl);
                }
                return Ok();
            }
        }
        catch (Exception ex)
        {
            if (ex is UserFriendlyException)
            {
                return Content(ex.Message);
            }
            else return Content(_i18n.T("UnknownException") ?? "UnknownException");
        }

        await _events.RaiseAsync(new UserLoginFailureEvent(inputModel.PhoneLogin ? inputModel.PhoneNumber : inputModel.Account,
                "invalid credentials", clientId: context?.Client.ClientId));
        return Content(_i18n.T("LoginValidateError") ?? "LoginValidateError");
    }

    /// <summary>
    /// Record login operation log (including client information)
    /// </summary>
    private async Task RecordLoginOperationLogAsync(UserModel user, string description, string? clientId)
    {
        try
        {
            var operationLogModel = new AddOperationLogModel(
                user.Id,
                user.DisplayName,
                OperationTypes.Login,
                DateTime.UtcNow,
                description,
                clientId);

            await _authClient.OperationLogService.AddLogAsync(operationLogModel);
        }
        catch (Exception ex)
        {
            // Logging failure should not affect login flow, only record errors
            _logger.LogError(ex, "Failed to record login operation log for user {UserId}", user.Id);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Logout(string logoutId = "")
    {
        var redirectUrl = "/Account/Logout/Loggedout";
        if (string.IsNullOrWhiteSpace(logoutId))
        {
            logoutId = await _interaction.CreateLogoutContextAsync();
        }
        redirectUrl = $"{redirectUrl}?logoutId={logoutId}";
        if (User.Identity != null && User.Identity.IsAuthenticated == true)
        {
            // delete local authentication cookie
            await HttpContext.SignOutAsync();

            foreach (var cookies in HttpContext.Request.Cookies)
            {
                if (cookies.Key == CookieKeyConfig.LangCookieKey)
                    continue;

                HttpContext.Response.Cookies.Delete(cookies.Key);
            }

            // raise the logout event
            await _events.RaiseAsync(new UserLogoutSuccessEvent(User.GetSubjectId(), User.GetDisplayName()));

            // see if we need to trigger federated logout
            var idp = User.FindFirst(JwtClaimTypes.IdentityProvider)?.Value;

            // if it's a local login we can ignore this workflow
            if (idp != null && idp != IdentityServerConstants.LocalIdentityProvider)
            {
                // we need to see if the provider supports external logout
                if (await HttpContext.GetSchemeSupportsSignOutAsync(idp))
                {
                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = redirectUrl }, idp);
                }
            }
        }
        return Redirect(redirectUrl);
    }

    [HttpGet]
    public async Task<IActionResult> LogoutUserAsync(string subjectId)
    {
        // TODO: 上线前恢复内网 IP 检查
        // var remoteIp = HttpContext.Connection.RemoteIpAddress;
        // if (remoteIp != null && !IsInternalNetwork(remoteIp))
        // {
        //     _logger.LogWarning("LogoutUserAsync rejected from external IP {RemoteIp}", remoteIp);
        //     return NotFound();
        // }

        _logger.LogInformation("Kicking user {SubjectId}", subjectId);

        var logoutSession = await _distributedCacheClient.GetAsync<LoginSession>(subjectId);

        var grants = await _persistedGrantService.GetAllGrantsAsync(subjectId);
        var clientIds = grants.Select(g => g.ClientId).Distinct().ToList();

        if (logoutSession != null && clientIds.Count > 0)
        {
            try
            {
                var logoutNotificationContext = new LogoutNotificationContext()
                {
                    SessionId = logoutSession.Sid,
                    SubjectId = subjectId,
                    ClientIds = clientIds,
                };
                await _backChannelClient.SendLogoutNotificationsAsync(logoutNotificationContext);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send backchannel logout notifications for subject {SubjectId}", subjectId);
            }
        }

        try
        {
            await _persistedGrantService.RemoveAllGrantsAsync(subjectId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to remove persisted grants for subject {SubjectId}", subjectId);
        }

        await _events.RaiseAsync(new UserLogoutSuccessEvent(subjectId, "Kicked by admin"));

        await _distributedCacheClient.SetAsync(
            $"kicked:{subjectId}", true, DateTimeOffset.UtcNow.AddHours(24));

        _logger.LogInformation("User {SubjectId} kicked successfully", subjectId);

        return Ok();
    }

    private static bool IsInternalNetwork(System.Net.IPAddress ip)
    {
        if (System.Net.IPAddress.IsLoopback(ip)) return true;

        if (ip.IsIPv4MappedToIPv6)
            ip = ip.MapToIPv4();

        var bytes = ip.GetAddressBytes();
        if (bytes.Length == 4)
        {
            // 10.0.0.0/8
            if (bytes[0] == 10) return true;
            // 172.16.0.0/12
            if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
            // 192.168.0.0/16
            if (bytes[0] == 192 && bytes[1] == 168) return true;
        }

        return false;
    }
}
