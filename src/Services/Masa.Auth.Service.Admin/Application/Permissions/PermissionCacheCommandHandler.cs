// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class PermissionCacheCommandHandler
{
    readonly IDistributedCacheClient _cacheClient;
    readonly IPermissionRepository _permissionRepository;

    public PermissionCacheCommandHandler(IDistributedCacheClient cacheClient, IPermissionRepository permissionRepository)
    {
        _cacheClient = cacheClient;
        _permissionRepository = permissionRepository;
    }

    [EventHandler(99)]
    public async Task UpsertPermissionAsync(UpsertPermissionCommand upsertPermissionCommand)
    {
        var cachePermission = upsertPermissionCommand.PermissionDetail.Adapt<CachePermission>();
        await _cacheClient.SetAsync(upsertPermissionCommand.PermissionDetail.Id.ToString(), cachePermission);

        var allPermissions = await _cacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (allPermissions == null)
        {
            allPermissions = new List<CachePermission>();
        }
        var existsData = allPermissions.FirstOrDefault(e => e.Id == cachePermission.Id);
        if (existsData != null)
        {
            allPermissions.Remove(existsData);
        }
        allPermissions.Add(cachePermission);
        await _cacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler(99)]
    public async Task RemovePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        await _cacheClient.RemoveAsync<CachePermission>(removePermissionCommand.PermissionId.ToString());

        var allPermissions = await _cacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (allPermissions == null)
        {
            allPermissions = new List<CachePermission>();
        }
        allPermissions = allPermissions.Where(e => e.Id != removePermissionCommand.PermissionId).ToList();
        await _cacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler(99)]
    public async Task SeedPermissionsAsync(SeedPermissionsCommand seedPermissionsCommand)
    {
        var allPermissions = new List<CachePermission>();
        foreach (var permission in seedPermissionsCommand.Permissions)
        {
            var cachePermission = permission.Adapt<CachePermission>();
            await _cacheClient.SetAsync(permission.Id.ToString(), cachePermission);
            allPermissions.Add(cachePermission);
        }
        await _cacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
    }

    [EventHandler]
    public async Task SyncPermissionsRedisAsync(SyncPermissionRedisCommand command)
    {
        var permissions = await _permissionRepository.GetAllAsync();
        var allPermissions = new List<CachePermission>();
        foreach (var permission in permissions)
        {
            var cachePermission = permission.Adapt<CachePermission>();
            await _cacheClient.SetAsync(permission.Id.ToString(), cachePermission);
            allPermissions.Add(cachePermission);
        }
        await _cacheClient.SetAsync(CacheKey.AllPermissionKey(), allPermissions);
        command.Count = permissions.Count();
    }

    [EventHandler]
    public async Task CollectMenuAsync(FavoriteMenuCommand favoriteMenuCommand)
    {
        var key = CacheKey.UserMenuCollectKey(favoriteMenuCommand.UserId);
        var menus = (await _cacheClient.GetAsync<HashSet<Guid>>(key)) ?? new HashSet<Guid>();
        if (favoriteMenuCommand.IsFavorite)
        {
            menus.Add(favoriteMenuCommand.PermissionId);
        }
        else
        {
            menus.Remove(favoriteMenuCommand.PermissionId);
        }
        await _cacheClient.SetAsync(key, menus);
    }
}
