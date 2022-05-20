// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Consent;

public class ConsentModel : PageModel
{
    readonly IIdentityServerInteractionService _interaction = null!;

    public ConsentModel(IIdentityServerInteractionService interaction)
    {
        _interaction = interaction;
    }

    [BindProperty]
    public InputModel Input { get; set; } = new();

    public async Task<IActionResult> OnGet(bool consent, string returnUrl)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);
        Input = new InputModel
        {
            ReturnUrl = returnUrl,
        };

        ConsentResponse? grantedConsent = null;

        if (consent)
        {
            var scopes = Input.ScopesConsented;
            if (ConsentOptions.EnableOfflineAccess == false)
            {
                scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
            }
            grantedConsent = new ConsentResponse
            {
                RememberConsent = Input.RememberConsent,
                ScopesValuesConsented = scopes.ToArray(),
                Description = Input.Description
            };
        }
        else
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };
        }

        if (grantedConsent != null)
        {
            // communicate outcome of consent back to identityserver
            await _interaction.GrantConsentAsync(request, grantedConsent);

            // redirect back to authorization endpoint
            if (request.IsNativeClient() == true)
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                return this.LoadingPage(returnUrl);
            }
        }
        return Redirect(returnUrl);
    }
}
