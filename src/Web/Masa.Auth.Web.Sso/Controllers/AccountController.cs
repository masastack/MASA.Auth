// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Controllers;

[Microsoft.AspNetCore.Mvc.Route("[action]")]
[AllowAnonymous]
[SecurityHeaders]
public class AccountController : Controller
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly IDistributedCacheClient _distributedCacheClient;
    readonly IUserSession _userSession;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IEventService events, IUserSession userSession,
        IAuthClient authClient,
        IDistributedCacheClient distributedCacheClient)
    {
        _interaction = interaction;
        _userSession = userSession;
        _events = events;
        _authClient = authClient;
        _distributedCacheClient = distributedCacheClient;
    }

    public async Task<List<string>> Test()
    {
        return (await _userSession.GetClientListAsync()).ToList();
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInputModel inputModel)
    {
        var returnUrl = inputModel.ReturnUrl;
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        try
        {
            var ssoEnvironmentProvider = HttpContext.RequestServices.GetService<IEnvironmentProvider>() as ISsoEnvironmentProvider;
            if (ssoEnvironmentProvider != null)
            {
                ssoEnvironmentProvider.SetEnvironment(inputModel.Environment);
            }

            var success = false;

            if (inputModel.PhoneLogin)
            {
                var key = CacheKey.GetSmsCodeKey(inputModel.PhoneNumber);
                var code = await _distributedCacheClient.GetAsync<int>(key);
                success = code == inputModel.SmsCode;
                await _distributedCacheClient.RemoveAsync<int>(key);
            }
            else
            {
                success = await _authClient.UserService
                                           .ValidateCredentialsByAccountAsync(inputModel.UserName, inputModel.Password, inputModel.LdapLogin);
            }

            if (success)
            {
                var user = new UserModel();
                if (inputModel.PhoneLogin)
                {
                    user = await _authClient.UserService.FindByPhoneNumberAsync(inputModel.PhoneNumber);
                    if (user is null)
                    {
                        //todo auto register user
                        return Content("no corresponding user for this mobile phone number");
                    }
                }
                else
                {
                    user = await _authClient.UserService.FindByAccountAsync(inputModel.UserName);
                }
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
                };
                var isuser = new IdentityServerUser(user!.Id.ToString())
                {
                    DisplayName = user.DisplayName
                };
                isuser.AdditionalClaims.Add(new Claim("userName", user.Account));
                isuser.AdditionalClaims.Add(new Claim("environment", inputModel.Environment));
                isuser.AdditionalClaims.Add(new Claim("role", JsonSerializer.Serialize(user.RoleIds)));

                //us sign in
                await HttpContext.SignInAsync(isuser, props);

                await _events.RaiseAsync(new UserLoginSuccessEvent(user.Name, user.Id.ToString(), user.DisplayName, clientId: context?.Client.ClientId));

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
            else return Content("Unknown exception,Please contact the administrator");
        }

        await _events.RaiseAsync(new UserLoginFailureEvent(inputModel.PhoneLogin ? inputModel.PhoneNumber : inputModel.UserName,
                "invalid credentials", clientId: context?.Client.ClientId));
        return Content("username and password validate error");
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
}
