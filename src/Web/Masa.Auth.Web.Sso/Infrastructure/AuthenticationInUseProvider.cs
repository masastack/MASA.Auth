// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class AuthenticationInUseProvider : IAuthenticationSchemeInUseProvider
{
    readonly IAuthClient _authClient;

    public AuthenticationInUseProvider(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<List<string>> GetAllSchemes()
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return thirdPartyIdps.Select(tpIdp => tpIdp.Name).ToList();
    }

    public async Task<List<AuthenticationDefaults>> GetAllAuthenticationDefaults()
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return thirdPartyIdps.Select(tpIdp => new AuthenticationDefaults 
        {
            Scheme = tpIdp.Name
        }).ToList();
    }
}
