// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class GitHubAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="GitHubAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables GitHub authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDefaultGitHub(this AuthenticationBuilder builder, Action<GitHubAuthenticationOptions> configuration)
    {
        configuration = options =>
        {
            configuration.Invoke(options);
            options.SignInScheme = AuthenticationExternalConstants.ExternalCookieAuthenticationScheme;
            options.Scope.Add("user:email");
            options.ClaimActions.MapJsonKey(UserClaims.WebSite, "html_url");
            options.ClaimActions.MapJsonKey(UserClaims.Picture, "avatar_url");
            options.ClaimActions.MapJsonKey(UserClaims.Account, "login");
        };
        return builder.AddGitHub(configuration);
    }
}
