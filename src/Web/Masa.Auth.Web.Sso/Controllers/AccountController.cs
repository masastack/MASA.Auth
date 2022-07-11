// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Controllers;

[Microsoft.AspNetCore.Mvc.Route("[action]")]
[Authorize]
public class AccountController : Controller
{
    readonly IAuthClient _authClient;
    readonly IIdentityServerInteractionService _interaction;
    readonly IEventService _events;

    public AccountController(
        IIdentityServerInteractionService interaction,
        IEventService events, IAuthClient authClient)
    {
        _interaction = interaction;
        _events = events;
        _authClient = authClient;
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] InputModel inputModel)
    {
        var returnUrl = inputModel.ReturnUrl;
        var userName = inputModel.UserName;
        // check if we are in the context of an authorization request
        var context = await _interaction.GetAuthorizationContextAsync(returnUrl);
        if (await _authClient.UserService.ValidateCredentialsByAccountAsync(userName, inputModel.Password))
        {
            var user = await _authClient.UserService.FindByAccountAsync(userName);
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
            isuser.AdditionalClaims.Add(new Claim("userName", userName));
            isuser.AdditionalClaims.Add(new Claim("environment", inputModel.Environment));
            isuser.AdditionalClaims.Add(new Claim("role", JsonSerializer.Serialize(user.Roles)));
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
        else
        {
            await _events.RaiseAsync(new UserLoginFailureEvent(userName, "invalid credentials", clientId: context?.Client.ClientId));
            return Content("username and password validate error");
        }
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
