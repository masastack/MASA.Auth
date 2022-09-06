// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using static AspNet.Security.OAuth.GitHub.GitHubAuthenticationConstants;

namespace Masa.Auth.Security.OAuth.Providers.GitHub;

public class GitHubBuilder : IIdentityBuilder, IAuthenticationDefaultBuilder
{
    public string Scheme { get; } = GitHubAuthenticationDefaults.AuthenticationScheme;

    public Identity BuildIdentity(ClaimsPrincipal principal)
    {
        var identity = Identity.CreaterDefault(principal);
        identity.NickName = principal.FindFirstValue(Claims.Name);
        identity.WebSite = principal.FindFirstValue(Claims.Url);

        return identity;
    }

    public AuthenticationDefaults BuildAuthenticationDefaults()
    {
        return new AuthenticationDefaults
        {
            Scheme = GitHubAuthenticationDefaults.AuthenticationScheme,
            DisplayName = GitHubAuthenticationDefaults.DisplayName,
            Icon = "https://masa-cdn-dev.oss-cn-hangzhou.aliyuncs.com/app.ico",
            CallbackPath = GitHubAuthenticationDefaults.CallbackPath,
            Issuer = GitHubAuthenticationDefaults.Issuer,
            AuthorizationEndpoint = GitHubAuthenticationDefaults.AuthorizationEndpoint,
            TokenEndpoint = GitHubAuthenticationDefaults.TokenEndpoint,
            UserInformationEndpoint = GitHubAuthenticationDefaults.UserInformationEndpoint
        };
    }
}
