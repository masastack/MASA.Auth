using Microsoft.Extensions.DependencyInjection;

namespace Masa.Contrib.BasicAbilities.Auth;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAuthSdkServer(this IServiceCollection services, Action<Options>? configure = null)
    {
        var options = new Options("http://localhost:8080/");
        //Todo default option

        configure?.Invoke(options);
        services.AddSingleton(options);
        services.AddScoped<AuthClient>();

        return services;
    }
}

