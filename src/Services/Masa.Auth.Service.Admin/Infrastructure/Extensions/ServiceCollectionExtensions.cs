namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAggregateFactory(this IServiceCollection services)
    {
        var facotries = Assembly.GetExecutingAssembly().GetTypes().Where(t => t.IsClass &&
                !t.IsGenericType && !t.IsAbstract && t.IsAssignableTo(typeof(IAggregateFactory)));

        foreach (var facotry in facotries)
        {
            services.TryAddScoped(facotry);
        }
        return services;
    }
}