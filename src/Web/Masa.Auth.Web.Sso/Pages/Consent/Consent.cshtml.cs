// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Consent;

public class ConsentModel : PageModel
{
    readonly IIdentityServerInteractionService _interaction = null!;
    readonly IEventService _events = null!;

    public ConsentModel(IIdentityServerInteractionService interaction, IEventService events)
    {
        _interaction = interaction;
        _events = events;
    }

    public async Task<IActionResult> OnGet(bool consent, string returnUrl, bool rememberConsent, List<string> scopes)
    {
        var request = await _interaction.GetAuthorizationContextAsync(returnUrl);

        ConsentResponse? grantedConsent = null;

        if (consent)
        {
            if (ConsentOptions.EnableOfflineAccess == false)
            {
                scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess).ToList();
            }
            grantedConsent = new ConsentResponse
            {
                RememberConsent = rememberConsent,
                ScopesValuesConsented = scopes.ToArray()
            };
            // emit event
            await _events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
        }
        else
        {
            grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };
            // emit event
            await _events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
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
        return LocalRedirect(returnUrl);
    }
}
