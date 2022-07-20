// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Global;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGlobalForServer(this IServiceCollection services)
    {
        services.AddScoped<GlobalConfig>();
        return services;
    }

    public static IServiceCollection AddGlobalForWasmAsync(this IServiceCollection services, string baseUri)
    {
        services.AddScoped<GlobalConfig>();
        return services;
    }
}
