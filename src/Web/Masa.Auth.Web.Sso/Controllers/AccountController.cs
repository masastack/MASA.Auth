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
    readonly I18n _i18n;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IEventService events,
        IAuthClient authClient,
        I18n i18n)
    {
        _interaction = interaction;
        _events = events;
        _authClient = authClient;
        _i18n = i18n;
    }

    [HttpPost]
    public async Task<IActionResult> Login([FromBody] LoginInputModel inputModel)
    {
        var returnUrl = inputModel.ReturnUrl;
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        try
        {
            HttpContext.UseEnvironmentIsolation(inputModel.Environment);

            UserModel? user = new();

            if (inputModel.PhoneLogin)
            {
                user = await _authClient.UserService.LoginByPhoneNumberAsync(new LoginByPhoneNumberModel
                {
                    PhoneNumber = inputModel.PhoneNumber,
                    Code = inputModel.SmsCode?.ToString() ?? throw new UserFriendlyException(_i18n.T("SmsRequired")),
                    RegisterLogin = inputModel.RegisterLogin
                });
                if (user is null)
                {
                    //todo auto register user
                    return Content("no corresponding user for this mobile phone number");
                }
            }
            else
            {
                user = await _authClient.UserService
                                           .ValidateCredentialsByAccountAsync(inputModel.UserName, inputModel.Password, inputModel.LdapLogin);
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
                };

                var isuser = new IdentityServerUser(user!.Id.ToString())
                {
                    DisplayName = user.DisplayName
                };

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
            else return Content(_i18n.T("UnknownException"));
        }

        await _events.RaiseAsync(new UserLoginFailureEvent(inputModel.PhoneLogin ? inputModel.PhoneNumber : inputModel.UserName,
                "invalid credentials", clientId: context?.Client.ClientId));
        return Content(_i18n.T("LoginValidateError"));
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
