// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AspNet.Security.OAuth.Weixin.WeixinAuthenticationConstants;

namespace Masa.Auth.Security.OAuth.Providers.Weixin;

public class WeixinBuilder : IIdentityBuilder, IAuthenticationDefaultBuilder
{
    public string Scheme { get; } = GitHubAuthenticationDefaults.AuthenticationScheme;

    public Identity BuilderIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);
        identity.Subject = principal.FindFirstValue(Claims.UnionId);
        identity.Picture = principal.FindFirstValue(Claims.HeadImgUrl);
        identity.Region = principal.FindFirstValue(Claims.Province);
        identity.Locality = principal.FindFirstValue(Claims.City);

        return identity;
    }

    public AuthenticationDefaults BuilderAuthenticationDefaults()
    {
        return new AuthenticationDefaults
        {
            Scheme = GitHubAuthenticationDefaults.AuthenticationScheme,
            DisplayName = GitHubAuthenticationDefaults.DisplayName,
            CallbackPath = GitHubAuthenticationDefaults.CallbackPath,
            Issuer = GitHubAuthenticationDefaults.Issuer,
            AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint,
            TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint,
            UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint
        };
    }
}
