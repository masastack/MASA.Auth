// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class RoleCacheCommandHandler
{
    readonly IMemoryCacheClient _memoryCacheClient;

    public RoleCacheCommandHandler(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    [EventHandler(99)]
    public async Task AddRoleAsync(AddRoleCommand addRoleCommand)
    {
        var cacheRole = addRoleCommand.Role.Adapt<CacheRole>();
        await _memoryCacheClient.SetAsync($"{CacheKey.ROLE_CACHE_KEY_PRE}{addRoleCommand.RoleId}", cacheRole);
    }

    [EventHandler(99)]
    public async Task UpdateRoleAsync(UpdateRoleCommand updateRoleCommand)
    {
        var cacheRole = updateRoleCommand.Role.Adapt<CacheRole>();
        await _memoryCacheClient.SetAsync($"{CacheKey.ROLE_CACHE_KEY_PRE}{updateRoleCommand.Role.Id}", cacheRole);
    }

    [EventHandler(99)]
    public async Task RemoveRoleAsync(RemoveRoleCommand removeRoleCommand)
    {
        await _memoryCacheClient.RemoveAsync<CachePermission>($"{CacheKey.ROLE_CACHE_KEY_PRE}{removeRoleCommand.Role.Id}");
    }
}
