// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiGateways(this IServiceCollection services, Action<AuthApiOptions> configure)
    {
        services.AddSingleton<IResponseMessage, AuthResponseMessage>();
        var options = new AuthApiOptions();
        configure.Invoke(options);
        services.AddSingleton(options);
        services.AddStackCaller(Assembly.Load("Masa.Auth.ApiGateways.Caller"), (serviceProvider) => { return new TokenProvider(); }, jwtTokenValidatorOptions =>
        {
            jwtTokenValidatorOptions.AuthorityEndpoint = options.AuthorityEndpoint;
        }, clientRefreshTokenOptions =>
        {
            clientRefreshTokenOptions.ClientId = options.ClientId;
            clientRefreshTokenOptions.ClientSecret = options.ClientSecret;
        });
        return services;
    }
}

