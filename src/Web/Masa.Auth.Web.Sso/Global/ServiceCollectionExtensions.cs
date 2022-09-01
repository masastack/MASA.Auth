// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Global;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGlobalForServer(this IServiceCollection services)
    {
        services.AddScoped<GlobalConfig>();

        return services;
    }
}
