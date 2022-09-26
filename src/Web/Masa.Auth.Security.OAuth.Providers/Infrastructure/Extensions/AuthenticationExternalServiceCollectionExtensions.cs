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

        return builder;
    }

    public static AuthenticationBuilder AddHotUpdateAuthenticationExternal<TAuthenticationExternalHandler, RemoteAuthenticationDefaultsProvider>(this IServiceCollection services) where TAuthenticationExternalHandler : class, IAuthenticationExternalHandler where RemoteAuthenticationDefaultsProvider : class, IRemoteAuthenticationDefaultsProvider
    {
        var builder = services.AddScoped<IAuthenticationExternalHandler, TAuthenticationExternalHandler>()
                             .AddSingleton<IRemoteAuthenticationDefaultsProvider, RemoteAuthenticationDefaultsProvider>()
                             .AddAuthentication()
                             .AddCookie(AuthenticationExternalConstants.ExternalCookieAuthenticationScheme);
        services.Replace(ServiceDescriptor.Singleton<IAuthenticationSchemeProvider, HotUpdateAuthenticationSchemeProvider>());
        services.Replace(ServiceDescriptor.Scoped<IAuthenticationHandlerProvider, HotUpdateAuthenticationHandlerProvider>());
        services.Replace(ServiceDescriptor.Scoped<ICookieAuthenticationHandler, HotUpdateAuthenticationHandlerProvider>());
        services.InjectForHotUpdate();

        return builder;
    }
}
