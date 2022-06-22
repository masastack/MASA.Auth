// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGlobalForServer(this IServiceCollection services)
    {
        services.AddMasaI18nForServer("wwwroot/i18n");
        services.AddScoped<GlobalConfig>();

        return services;
    }

    public static async Task<IServiceCollection> AddGlobalForWasmAsync(this IServiceCollection services, string baseUri)
    {
        await services.AddMasaI18nForWasmAsync(Path.Combine(baseUri, $"i18n"));
        using var httpclient = new HttpClient();
        services.AddScoped<GlobalConfig>();

        return services;
    }
}
