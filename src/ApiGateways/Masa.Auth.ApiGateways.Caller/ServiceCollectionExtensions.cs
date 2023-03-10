// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiGateways(this IServiceCollection services, Action<AuthApiOptions>? configure = null)
    {
        services.AddSingleton<IResponseMessage, AuthResponseMessage>();
        var options = new AuthApiOptions();
        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddAutoRegistrationCaller(Assembly.Load("Masa.Auth.ApiGateways.Caller"));
        return services;
    }
}

