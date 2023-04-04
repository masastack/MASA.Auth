// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandlers;

public class UserAuthorizedDomainEventHandler
{
    readonly IPermissionRepository _permissionRepository;
    readonly IEventBus _eventBus;
    readonly AuthDbContext _authDbContext;
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public UserAuthorizedDomainEventHandler(
        IPermissionRepository permissionRepository,
        IEventBus eventBus,
        AuthDbContext authDbContext,
        IMultilevelCacheClient multilevelCacheClient)
    {
        _permissionRepository = permissionRepository;
        _eventBus = eventBus;
        _authDbContext = authDbContext;
        _multilevelCacheClient = multilevelCacheClient;
    }

    [EventHandler(1)]
    public async Task GetAuthorizedPermissionAsync(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var cachePermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        var permission = cachePermissions?.FirstOrDefault(p => p.AppId == userAuthorizedDomainEvent.AppId && p.Code == userAuthorizedDomainEvent.Code);
        if (permission is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSION_APP_ID_CODE_NOT_FOUND);
        }
        userAuthorizedDomainEvent.PermissionId = permission.Id;
    }

    [EventHandler(2)]
    public async Task AuthorizedPermissionsAsync(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var query = new PermissionsByUserQuery(userAuthorizedDomainEvent.UserId);
        await _eventBus.PublishAsync(query);

        userAuthorizedDomainEvent.Authorized = query.Result.Contains(userAuthorizedDomainEvent.PermissionId);
    }
}
