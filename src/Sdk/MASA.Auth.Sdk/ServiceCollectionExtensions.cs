using Microsoft.Extensions.DependencyInjection;

namespace Masa.Auth.Sdk;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthSdkServer(this IServiceCollection services)
    {
        services.AddScoped<AuthClient>();

        return services;
    }
}

