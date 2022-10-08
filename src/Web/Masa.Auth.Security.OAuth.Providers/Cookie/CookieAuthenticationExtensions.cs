// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class CookieAuthenticationExtensions
{
    public static AuthenticationBuilder AddCookieExternal(this AuthenticationBuilder builder, string scheme, string? displayName = null)
    {
        builder.Services.AddOptions<CookieAuthenticationOptions>(scheme).Validate(o => o.Cookie.Expiration == null, "Cookie.Expiration is ignored, use ExpireTimeSpan instead.");
        return builder.AddScheme<CookieAuthenticationOptions, Masa.Auth.Security.OAuth.Providers.CookieAuthenticationHandler>(scheme, displayName, default);
    }
}
