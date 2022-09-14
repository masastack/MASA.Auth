// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class RemoteAuthenticationDefaultsProvider : IRemoteAuthenticationDefaultsProvider
{
    readonly IAuthClient _authClient;

    public RemoteAuthenticationDefaultsProvider(IAuthClient authClient)
    {
        _authClient = authClient;
    }

    public async Task<AuthenticationDefaults?> GetAsync(string scheme)
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return Convert(thirdPartyIdps.FirstOrDefault(tpIdp => tpIdp.Name == scheme));
    }

    public async Task<List<AuthenticationDefaults>> GetAllAsync()
    {
        var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return thirdPartyIdps.Select(tpIdp => Convert(tpIdp)).ToList();
    }

    [return: NotNullIfNotNull("model")]
    AuthenticationDefaults? Convert(ThirdPartyIdpModel? model)
    {
        if (model is null) return null;

        return new AuthenticationDefaults
        {
            Scheme = model.Name,
            DisplayName = model.DisplayName,
            CallbackPath = model.CallbackPath,
            Issuer = model.Name,
            AuthorizationEndpoint = model.AuthorizationEndpoint,
            TokenEndpoint = model.TokenEndpoint,
            UserInformationEndpoint = model.UserInformationEndpoint,
            Icon = model.Icon,
            MapAll = model.MapAll,
            JsonKeyMap = model.JsonKeyMap
        };
    }
}
