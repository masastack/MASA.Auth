// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class PermissionCacheCommandHandler
{
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public PermissionCacheCommandHandler(IMultilevelCacheClient multilevelCacheClient)
    {
        _multilevelCacheClient = multilevelCacheClient;
    }

    [EventHandler(99)]
    public async Task AddPermissionAsync(AddPermissionCommand addPermissionCommand)
    {
        var cachePermission = addPermissionCommand.PermissionDetail.Adapt<CachePermission>();
        await _multilevelCacheClient.SetAsync(CacheKey.PermissionKey(addPermissionCommand.PermissionDetail.Id), cachePermission);

        var allPermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (allPermissions == null)
        {
            allPermissions = new List<CachePermission>();
        }
        allPermissions.Add(cachePermission);
        await _multilevelCacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler(99)]
    public async Task RemovePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        await _multilevelCacheClient.RemoveAsync<CachePermission>(CacheKey.PermissionKey(removePermissionCommand.PermissionId));

        var allPermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (allPermissions == null)
        {
            allPermissions = new List<CachePermission>();
        }
        allPermissions = allPermissions.Where(e => e.Id != removePermissionCommand.PermissionId).ToList();
        await _multilevelCacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler(99)]
    public async Task SeedPermissionsAsync(SeedPermissionsCommand seedPermissionsCommand)
    {
        var allPermissions = new List<CachePermission>();
        foreach (var permission in seedPermissionsCommand.Permissions)
        {
            var cachePermission = permission.Adapt<CachePermission>();
            await _multilevelCacheClient.SetAsync(CacheKey.PermissionKey(permission.Id), cachePermission);
            allPermissions.Add(cachePermission);
        }
        await _multilevelCacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler]
    public async Task CollectMenuAsync(FavoriteMenuCommand favoriteMenuCommand)
    {
        var key = CacheKey.UserMenuCollectKey(favoriteMenuCommand.UserId);
        var menus = (await _multilevelCacheClient.GetAsync<HashSet<Guid>>(key)) ?? new HashSet<Guid>();
        if (favoriteMenuCommand.IsFavorite)
        {
            menus.Add(favoriteMenuCommand.PermissionId);
        }
        else
        {
            menus.Remove(favoriteMenuCommand.PermissionId);
        }
        await _multilevelCacheClient.SetAsync(key, menus);
    }
}
