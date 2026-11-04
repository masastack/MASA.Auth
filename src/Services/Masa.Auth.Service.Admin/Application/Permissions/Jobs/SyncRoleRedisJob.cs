// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Jobs;

public class SyncRoleRedisJob : BackgroundJobBase<SyncRoleRedisArgs>
{
    readonly IServiceProvider _serviceProvider;

    public SyncRoleRedisJob(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecutingAsync(SyncRoleRedisArgs args)
    {
        await using var scope = _serviceProvider.CreateAsyncScope();
        var (cacheClient, roleRepository) = await GetRequiredServiceAsync(scope.ServiceProvider, args.Environment);

        var roles = await roleRepository.GetListWithPermissionsAsync();

        //批量写入：单条 SetAsync = SET+PUB 两次串行往返；SetListAsync 一次脚本完成
        var keyValues = roles.ToDictionary(
            role => CacheKey.RoleKey(role.Id),
            role => (CacheRole?)new CacheRole
            {
                Name = role.Name,
                Description = role.Description,
                Enabled = role.Enabled,
                Limit = role.Limit,
                Permissions = role.Permissions.Select(x => new SubjectPermissionRelationDto(x.PermissionId, x.Effect)).ToList(),
                ChildrenRoles = role.ChildrenRoles.Select(x => x.RoleId).ToList()
            });
        await cacheClient.SetListAsync(keyValues);
    }

    private async Task<(IDistributedCacheClient, IRoleRepository)> GetRequiredServiceAsync(IServiceProvider serviceProvider, string environment)
    {
        var multiEnvironmentSetter = serviceProvider.GetRequiredService<IMultiEnvironmentSetter>();
        multiEnvironmentSetter.SetEnvironment(environment);
        var cacheClient = serviceProvider.GetRequiredService<IDistributedCacheClient>();
        var roleRepository = serviceProvider.GetRequiredService<IRoleRepository>();
       
        return (cacheClient, roleRepository);
    }
}
