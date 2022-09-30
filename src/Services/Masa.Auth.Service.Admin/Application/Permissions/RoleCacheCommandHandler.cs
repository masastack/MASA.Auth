// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class RoleCacheCommandHandler
{
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public RoleCacheCommandHandler(IMultilevelCacheClient multilevelCacheClient)
    {
        _multilevelCacheClient = multilevelCacheClient;
    }

    [EventHandler(99)]
    public async Task AddRoleAsync(AddRoleCommand addRoleCommand)
    {
        var cacheRole = addRoleCommand.Role.Adapt<CacheRole>();
        await _multilevelCacheClient.SetAsync(CacheKey.RoleKey(addRoleCommand.Result.Id), cacheRole);
    }

    [EventHandler(99)]
    public async Task UpdateRoleAsync(UpdateRoleCommand updateRoleCommand)
    {
        var cacheRole = updateRoleCommand.Role.Adapt<CacheRole>();
        await _multilevelCacheClient.SetAsync(CacheKey.RoleKey(updateRoleCommand.Role.Id), cacheRole);
    }

    [EventHandler(99)]
    public async Task RemoveRoleAsync(RemoveRoleCommand removeRoleCommand)
    {
        await _multilevelCacheClient.RemoveAsync<CachePermission>(CacheKey.RoleKey(removeRoleCommand.Role.Id));
    }
}
