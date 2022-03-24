using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Masa.Auth.ApiGateways.Caller;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthApiGateways(this IServiceCollection services, Action<AuthApiOptions>? configure = null)
    {
        var options = new AuthApiOptions("http://localhost:8080/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddScoped<HttpClientAuthorizationDelegatingHandler>();
        services.AddCaller(Assembly.Load("Masa.Auth.ApiGateways.Caller"));
        return services;
    }
}

