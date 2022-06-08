// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandler;

public class UserAuthorizedDomainEventHandler
{
    readonly IPermissionRepository _permissionRepository;
    readonly IEventBus _eventBus;
    readonly AuthDbContext _authDbContext;

    public UserAuthorizedDomainEventHandler(
        IPermissionRepository permissionRepository,
        IEventBus eventBus,
        AuthDbContext authDbContext)
    {
        _permissionRepository = permissionRepository;
        _eventBus = eventBus;
        _authDbContext = authDbContext;
    }

    [EventHandler(1)]
    public async Task GetAuthorizedPermissionAsync(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var permission = await _permissionRepository.FindAsync(p => p.AppId == userAuthorizedDomainEvent.AppId
                            && p.Code == userAuthorizedDomainEvent.Code);
        if (permission is null)
        {
            throw new UserFriendlyException("AppId combination Code is not found.");
        }
        userAuthorizedDomainEvent.PermissionId = permission.Id;
        userAuthorizedDomainEvent.Roles = _authDbContext.Set<UserRole>()
                .Where(ur => ur.UserId == userAuthorizedDomainEvent.UserId && !ur.IsDeleted)
                .Select(ur => ur.RoleId).Distinct().ToList();
    }

    [EventHandler(4)]
    public async Task AuthorizedRolePermissionAsync(UserAuthorizedDomainEvent userAuthorizedDomainEvent)
    {
        var query = new PermissionsByRoleQuery(userAuthorizedDomainEvent.Roles);
        await _eventBus.PublishAsync(query);
        userAuthorizedDomainEvent.Authorized = query.Result.Contains(userAuthorizedDomainEvent.PermissionId)
                                                || userAuthorizedDomainEvent.Authorized;
    }
}
