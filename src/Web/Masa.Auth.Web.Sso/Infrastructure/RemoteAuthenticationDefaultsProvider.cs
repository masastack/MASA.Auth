// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using AspNet.Security.OAuth.GitHub;

namespace Masa.Auth.Web.Sso.Infrastructure;

public class RemoteAuthenticationDefaultsProvider : IRemoteAuthenticationDefaultsProvider
{
    //readonly IAuthClient _authClient;

    //public RemoteAuthenticationDefaultsProvider(IAuthClient authClient)
    //{
    //    _authClient = authClient;
    //}

    public List<ThirdPartyIdpModel> thirdPartyIdps = new List<ThirdPartyIdpModel>
    {
        new ThirdPartyIdpModel
        {
            Name = GitHubAuthenticationDefaults.AuthenticationScheme,
            DisplayName = GitHubAuthenticationDefaults.DisplayName,
            Icon = "https://masa-cdn-dev.oss-cn-hangzhou.aliyuncs.com/app.ico",
            CallbackPath = GitHubAuthenticationDefaults.CallbackPath,
            AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint,
            TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint,
            UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint,
            ClientId = "49e302895d8b09ea5656",
            ClientSecret ="98f1bf028608901e9df91d64ee61536fe562064b",
            JsonKeyMap = new Dictionary<string, string>
            {
                [UserClaims.WebSite] = "html_url",
                [UserClaims.Picture] = "avatar_url",
                [UserClaims.Account] = "login"
            }
        }
    };

    public async Task<AuthenticationDefaults?> GetAsync(string scheme)
    {
        //var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
        return Convert(thirdPartyIdps.FirstOrDefault(tpIdp => tpIdp.Name == scheme));
    }

    public async Task<List<AuthenticationDefaults>> GetAllAsync()
    {
        //var thirdPartyIdps = await _authClient.ThirdPartyIdpService.GetAllThirdPartyIdpAsync();
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
        };
    }
}
