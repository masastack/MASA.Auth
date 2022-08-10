// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;
using Masa.Utils.Caching.Core.Interfaces;

namespace Masa.Auth.Web.Sso.Controllers;

[Microsoft.AspNetCore.Mvc.Route("[action]")]
[Authorize]
[SecurityHeaders]
public class AccountController : Controller
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;
    readonly IDistributedCacheClient _distributedCacheClient;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IEventService events,
        IAuthClient authClient,
        IDistributedCacheClient distributedCacheClient)
    {
        _interaction = interaction;
        _events = events;
        _authClient = authClient;
        _distributedCacheClient = distributedCacheClient;
    }

    [HttpPost]
    [AllowAnonymous]
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
                var d = await _distributedCacheClient.GetAsync<int>(CacheKey.GetSmsCodeKey(inputModel.PhoneNumber));
                success = d == inputModel.SmsCode;
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
                var isuser = new IdentityServerUser(user.Id.ToString())
                {
                    DisplayName = user.DisplayName
                };
                isuser.AdditionalClaims.Add(new Claim("userName", user.Account));
                isuser.AdditionalClaims.Add(new Claim("environment", inputModel.Environment));
                isuser.AdditionalClaims.Add(new Claim("role", JsonSerializer.Serialize(user.RoleIds)));
                //us duende sign in
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
        if (User.Identity != null && User.Identity.IsAuthenticated == true)
        {
            if (string.IsNullOrEmpty(logoutId))
            {
                logoutId = await _interaction.CreateLogoutContextAsync();
            }
            // delete local authentication cookie
            await HttpContext.SignOutAsync();

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
                    // build a return URL so the upstream provider will redirect back
                    // to us after the user has logged out. this allows us to then
                    // complete our single sign-out processing.
                    string url = $"/Account/Logout/Loggedout?logoutId={logoutId}";

                    // this triggers a redirect to the external provider for sign-out
                    return SignOut(new AuthenticationProperties { RedirectUri = url }, idp);
                }
            }
        }
        return Redirect($"/Account/Logout/Loggedout?logoutId={logoutId}");
    }
}
