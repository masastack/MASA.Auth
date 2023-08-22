// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure.Validations;

public class ThirdPartyIdpGrantValidator : IExtensionGrantValidator
{
    IAuthClient _authClient;
    IRemoteAuthenticationDefaultsProvider _remoteAuthenticationDefaultsProvider;
    ThirdPartyIdpCallerProvider _thirdPartyIdpCallerProvider;

    public string GrantType { get; } = BuildingBlocks.Authentication.OpenIdConnect.Models.Constans.GrantType.THIRD_PARTY_IDP;

    public ThirdPartyIdpGrantValidator(IAuthClient authClient, IRemoteAuthenticationDefaultsProvider remoteAuthenticationDefaultsProvider, ThirdPartyIdpCallerProvider thirdPartyIdpCallerProvider)
    {
        _authClient = authClient;
        _remoteAuthenticationDefaultsProvider = remoteAuthenticationDefaultsProvider;
        _thirdPartyIdpCallerProvider = thirdPartyIdpCallerProvider;
    }

    public async Task ValidateAsync(ExtensionGrantValidationContext context)
    {
        var scheme = context.Request.Raw["scheme"];
        var code = context.Request.Raw["code"];
        var idToken = context.Request.Raw["idToken"];
        if (string.IsNullOrEmpty(scheme))
        {
            throw new UserFriendlyException("must provider scheme");
        }
        var authenticationDefaults = await _remoteAuthenticationDefaultsProvider.GetAsync(scheme) ?? throw new UserFriendlyException($"No {scheme} configuration information found");
        Security.OAuth.Providers.Identity identity;
        if (string.IsNullOrEmpty(code) is false)
        {
            identity = await _thirdPartyIdpCallerProvider.GetIdentity(authenticationDefaults, code);
        }
        else
        {
            if (string.IsNullOrEmpty(idToken))
            {
                throw new UserFriendlyException("must provider code or idToken");
            }
            identity = await _thirdPartyIdpCallerProvider.GetIdentityByIdToken(authenticationDefaults, idToken);
        }
        var user = await _authClient.UserService.GetThirdPartyUserAsync(new GetThirdPartyUserModel
        {
            ThridPartyIdentity = identity.Subject
        });
        context.Result = new GrantValidationResult(user?.Id.ToString() ?? "", "thirdPartyIdp", customResponse: new()
        {
            ["thirdPartyUserData"] = identity,
            ["registerSuccess"] = user is not null
        });
    }
}
