// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Infrastructure;

public class RemoteAuthenticationDefaultsProvider : IRemoteAuthenticationDefaultsProvider
{
    readonly IThirdPartyIdpService _thirdPartyIdpService;

    public RemoteAuthenticationDefaultsProvider(IServiceProvider serviceProvider)
    {
        _thirdPartyIdpService = serviceProvider.CreateScope().ServiceProvider.GetRequiredService<IThirdPartyIdpService>();
    }
    //public RemoteAuthenticationDefaultsProvider(IThirdPartyIdpService thirdPartyIdpService)
    //{
    //    _thirdPartyIdpService = thirdPartyIdpService;
    //}

    public async Task<AuthenticationDefaults?> GetAsync(string scheme)
    {
        var thirdPartyIdps = await _thirdPartyIdpService.GetAllFromCacheAsync();
        return Convert(thirdPartyIdps.FirstOrDefault(tpIdp => tpIdp.Name == scheme));
    }

    public async Task<List<AuthenticationDefaults>> GetAllAsync()
    {
        var thirdPartyIdps = await _thirdPartyIdpService.GetAllFromCacheAsync();
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
            JsonKeyMap = model.JsonKeyMap,
            ClientSecret = model.ClientSecret,
            ClientId = model.ClientId,
            ThirdPartyIdpType = model.ThirdPartyIdpType,
        };
    }
}
