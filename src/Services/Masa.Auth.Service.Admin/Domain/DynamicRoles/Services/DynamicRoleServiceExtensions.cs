// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.DynamicRoles.Services;

/// <summary>
/// Dynamic role service extension methods for dependency injection configuration
/// </summary>
public static class DynamicRoleServiceExtensions
{
    /// <summary>
    /// Register dynamic role related services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <returns>Service collection</returns>
    public static IServiceCollection AddDynamicRoleServices(this IServiceCollection services)
    {
        // Register field value extractors
        services.AddScoped<IFieldValueExtractor, UserInfoFieldValueExtractor>();
        services.AddScoped<IFieldValueExtractor, UserClaimFieldValueExtractor>();
        services.AddScoped<IFieldValueExtractor, RoleFieldValueExtractor>();
        // Register dynamic role field value extractor with special handling for circular dependencies
        services.AddScoped<IFieldValueExtractor>(provider =>
        {
            var repository = provider.GetRequiredService<IDynamicRoleRepository>();
            // Use lazy resolution to break circular dependency
            Func<User, DynamicRole, Task<bool>> evaluateFunc = async (user, role) =>
            {
                var dynamicRoleService = provider.GetRequiredService<DynamicRoleService>();
                return await dynamicRoleService.EvaluateConditionsAsync(user, role);
            };
            return new DynamicRoleFieldValueExtractor(repository, evaluateFunc);
        });

        // Register factory
        services.AddScoped<FieldValueExtractorFactory>();

        return services;
    }
}