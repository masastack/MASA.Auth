// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class RoleCacheCommandHandler
{
    readonly IDistributedCacheClient _cacheClient;
    readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public RoleCacheCommandHandler(IDistributedCacheClient cacheClient, IMultiEnvironmentContext multiEnvironmentContext)
    {
        _cacheClient = cacheClient;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler(99)]
    public async Task AddRoleAsync(AddRoleCommand addRoleCommand)
    {
        var cacheRole = addRoleCommand.Role.Adapt<CacheRole>();
        await _cacheClient.SetAsync(CacheKey.RoleKey(addRoleCommand.Result.Id), cacheRole);
    }

    [EventHandler(99)]
    public async Task UpdateRoleAsync(UpdateRoleCommand updateRoleCommand)
    {
        var cacheRole = updateRoleCommand.Role.Adapt<CacheRole>();
        await _cacheClient.SetAsync(CacheKey.RoleKey(updateRoleCommand.Role.Id), cacheRole);
    }

    [EventHandler(99)]
    public async Task RemoveRoleAsync(RemoveRoleCommand removeRoleCommand)
    {
        await _cacheClient.RemoveAsync<CachePermission>(CacheKey.RoleKey(removeRoleCommand.Role.Id));
    }


    [EventHandler]
    public async Task SyncRoleRedisAsync(SyncRoleRedisCommand command)
    {
        var args = new SyncRoleRedisArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }
}
