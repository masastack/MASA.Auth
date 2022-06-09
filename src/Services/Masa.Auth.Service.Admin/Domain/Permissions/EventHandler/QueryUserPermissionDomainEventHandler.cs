// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandler;

public class QueryUserPermissionDomainEventHandler
{
    readonly IEventBus _eventBus;

    public QueryUserPermissionDomainEventHandler(
        IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    [EventHandler(4)]
    public async Task UserRolePermissionAsync(QueryUserPermissionDomainEvent queryUserPermissionDomainEvent)
    {
        var query = new PermissionsByRoleQuery(queryUserPermissionDomainEvent.Roles);
        await _eventBus.PublishAsync(query);
        queryUserPermissionDomainEvent.Permissions = queryUserPermissionDomainEvent.Permissions.Union(query.Result).ToList();
    }
}