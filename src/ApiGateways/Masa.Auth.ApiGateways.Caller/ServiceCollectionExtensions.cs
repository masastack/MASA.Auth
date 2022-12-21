// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiGateways(this IServiceCollection services, Action<AuthApiOptions>? configure = null)
    {
        services.AddSingleton<IResponseMessage, AuthResponseMessage>();
        var options = new AuthApiOptions("http://localhost:18002/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddCaller(Assembly.Load("Masa.Auth.ApiGateways.Caller"));
        return services;
    }

    public static IServiceCollection AddJwtTokenValidator(this IServiceCollection services,
        Action<JwtTokenValidatorOptions> jwtTokenValidatorOptions, Action<ClientRefreshTokenOptions> clientRefreshTokenOptions)
    {
        var options = new JwtTokenValidatorOptions();
        jwtTokenValidatorOptions.Invoke(options);
        services.AddSingleton(options);
        var refreshTokenOptions = new ClientRefreshTokenOptions();
        clientRefreshTokenOptions.Invoke(refreshTokenOptions);
        services.AddSingleton(refreshTokenOptions);
        services.AddScoped<JwtTokenValidator>();
        return services;
    }
}

