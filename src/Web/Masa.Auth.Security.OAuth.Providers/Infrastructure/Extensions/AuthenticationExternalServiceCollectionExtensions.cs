// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Microsoft.Extensions.DependencyInjection;

public static class AuthenticationExternalServiceCollectionExtensions
{
    public static AuthenticationBuilder AddAuthenticationExternal<TAuthenticationExternalHandler>(this IServiceCollection services) where TAuthenticationExternalHandler : class, IAuthenticationExternalHandler
    {
        var builder = services.AddScoped<IAuthenticationExternalHandler, TAuthenticationExternalHandler>()
                              .AddAuthentication()
                              .AddCookie(AuthenticationExternalConstants.ExternalCookieAuthenticationScheme);
        services.Replace(ServiceDescriptor.Singleton<IAuthenticationSchemeProvider, HotUpdateAuthenticationSchemeProvider>());
        services.Replace(ServiceDescriptor.Scoped<IAuthenticationHandlerProvider, HotUpdateAuthenticationHandlerProvider>());

        return builder;
    }
}
