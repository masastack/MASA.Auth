// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class AuthenticationInUseProvider : IRemoteAuthenticationDefaultsProvider
{
    readonly IAuthClient _authClient;

    public AuthenticationInUseProvider(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<List<string>> GetAllSchemesAsync()
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return thirdPartyIdps.Select(tpIdp => tpIdp.Name).ToList();
    }

    public async Task<List<AuthenticationDefaults>> GetAllAuthenticationDefaultsAsync()
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return thirdPartyIdps.Select(tpIdp => new AuthenticationDefaults 
        {
            Scheme = tpIdp.Name,
            DisplayName = tpIdp.DisplayName,
            CallbackPath = tpIdp.CallbackPath,
            Issuer = tpIdp.Name,
            AuthorizationEndpoint = tpIdp.AuthorizationEndpoint,
            TokenEndpoint = tpIdp.TokenEndpoint,
            UserInformationEndpoint = tpIdp.UserInformationEndpoint,
            Icon = tpIdp.Icon,
            MapAll = tpIdp.MapAll,
            JsonKeyMap = tpIdp.JsonKeyMap
        }).ToList();
    }
}
