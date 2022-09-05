// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class WeixinAuthenticationExtensions
{
    /// <summary>
    /// Adds <see cref="GitHubAuthenticationHandler"/> to the specified
    /// <see cref="AuthenticationBuilder"/>, which enables GitHub authentication capabilities.
    /// </summary>
    /// <param name="builder">The authentication builder.</param>
    /// <returns>The <see cref="AuthenticationBuilder"/>.</returns>
    public static AuthenticationBuilder AddDefaulrWeixin(this AuthenticationBuilder builder, Action<WeixinAuthenticationOptions> configuration)
    {
        configuration = options =>
        {
            configuration.Invoke(options);
            options.SignInScheme = AuthenticationExternalConstants.ExternalCookieAuthenticationScheme;
        };
        return builder.AddWeixin(configuration);
    }
}
