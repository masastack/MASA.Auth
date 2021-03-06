// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class PermissionCacheCommandHandler
{
    readonly IMemoryCacheClient _memoryCacheClient;

    public PermissionCacheCommandHandler(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    [EventHandler(99)]
    public async Task AddPermissionAsync(AddPermissionCommand addPermissionCommand)
    {
        var cachePermission = addPermissionCommand.PermissionDetail.Adapt<CachePermission>();
        cachePermission.ApiPermissions = addPermissionCommand.ApiPermissions;
        cachePermission.ParentId = addPermissionCommand.ParentId;
        await _memoryCacheClient.SetAsync(CacheKey.PermissionKey(addPermissionCommand.PermissionDetail.Id), cachePermission);
    }

    [EventHandler(99)]
    public async Task RemovePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        await _memoryCacheClient.RemoveAsync<CachePermission>(CacheKey.PermissionKey(removePermissionCommand.PermissionId));
    }

    [EventHandler]
    public async Task CollectMenuAsync(FavoriteMenuCommand favoriteMenuCommand)
    {
        var key = CacheKey.UserMenuCollectKey(favoriteMenuCommand.UserId);
        var menus = (await _memoryCacheClient.GetAsync<HashSet<Guid>>(key)) ?? new HashSet<Guid>();
        if (favoriteMenuCommand.IsFavorite)
        {
            menus.Add(favoriteMenuCommand.PermissionId);
        }
        else
        {
            menus.Remove(favoriteMenuCommand.PermissionId);
        }
        await _memoryCacheClient.SetAsync(key, menus);
    }
}
