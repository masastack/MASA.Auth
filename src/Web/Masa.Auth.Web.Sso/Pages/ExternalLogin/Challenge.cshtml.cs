// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.ExternalLogin;

[AllowAnonymous]
public class ChallengeModel : PageModel
{
    readonly IIdentityServerInteractionService _interactionService;

    public ChallengeModel(IIdentityServerInteractionService interactionService)
    {
        _interactionService = interactionService;
    }

    public IActionResult OnGet(string scheme, string returnUrl)
    {
        if (string.IsNullOrEmpty(returnUrl))
        {
            returnUrl = "~/";
        }

        // validate returnUrl - either it is a valid OIDC URL or back to a local page
        if (!Url.IsLocalUrl(returnUrl) && !_interactionService.IsValidReturnUrl(returnUrl))
        {
            throw new Exception("invalid return URL");
        }

        // start challenge and roundtrip the return URL and scheme 
        var props = new AuthenticationProperties
        {
            RedirectUri = Url.Page("/externallogin/callback"),
            Items =
            {
                { "returnUrl", returnUrl },
                { "scheme", scheme },
            }
        };

        return Challenge(props, scheme);
    }
}
