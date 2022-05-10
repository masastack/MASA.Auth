// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Controllers;

[Microsoft.AspNetCore.Mvc.Route("[action]")]
public class AccountController : Controller
{
    readonly TestUserStore _users;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;

    public AccountController(TestUserStore users, IIdentityServerInteractionService interaction, IEventService events)
    {
        _users = users;
        _interaction = interaction;
        _events = events;
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Login(string userName, string password, bool rememberLogin = false, string returnUrl = "")
    {
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);

        if (_users.ValidateCredentials(userName, password))
        {
            var user = _users.FindByUsername(userName);
            // only set explicit expiration here if user chooses "remember me". 
            // otherwise we rely upon expiration configured in cookie middleware.
            AuthenticationProperties? props = null;
            if (LoginOptions.AllowRememberLogin && rememberLogin)
            {
                props = new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.Add(LoginOptions.RememberMeLoginDuration)
                };
            };
            var isuser = new IdentityServerUser(user.SubjectId)
            {
                DisplayName = user.Username
            };
            //us duende sign in
            await HttpContext.SignInAsync(isuser, props);

            await _events.RaiseAsync(new UserLoginSuccessEvent(user.Username, user.SubjectId, user.Username, clientId: context?.Client.ClientId));
            if (context != null)
            {
                if (context.IsNativeClient())
                {
                    // The client is native, so this change in how to
                    // return the response is for better UX for the end user.

#warning
                    //Navigation.LoadingPage(_inputModel.ReturnUrl);
                }

                // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                return Redirect(returnUrl);
            }

            // request for a local page
            if (UrlHelper.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else if (string.IsNullOrEmpty(returnUrl))
            {
                return Redirect("/");
            }
            else
            {
                // user might have clicked on a malicious link - should be logged
                throw new Exception("invalid return URL");
            }
        }
        else
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(userName, "invalid credentials", clientId: context?.Client.ClientId));
            throw new UserFriendlyException("username and password validate error");
        }
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Logout(string logoutId = "")
    {
        if (User.Identity != null && User.Identity.IsAuthenticated == true)
        {
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
        return RedirectToPage("/Account/Logout/LoggedOut", new { logoutId = logoutId });
    }
}
