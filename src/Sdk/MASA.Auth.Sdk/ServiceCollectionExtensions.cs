namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthSdkServer(this IServiceCollection services)
    {
        services.AddScoped<AuthClient>();

        return services;
    }
}

