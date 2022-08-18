// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    public UserDomainService(IDomainEventBus eventBus) : base(eventBus)
    {
    }

    public async Task AddAsync(User user)
    {
        await EventBus.PublishAsync(new AddUserDomainEvent(user));
    }

    public async Task UpdateAsync(User user)
    {
        await EventBus.PublishAsync(new UpdateUserDomainEvent(user));
    }

    public async Task RemoveAsync(User user)
    {
        await EventBus.PublishAsync(new RemoveUserDomainEvent(user));
    }

    public async Task UpdateAuthorizationAsync(IEnumerable<Guid> roles)
    {
        await EventBus.PublishAsync(new UpdateUserAuthorizationDomainEvent(roles));
    }


    public async Task<List<Guid>> GetPermissionIdsAsync(Guid userId, List<Guid>? teams = null)
    {
        var query = new QueryUserPermissionDomainEvent(userId, teams);
        await EventBus.PublishAsync(query);
        return query.Permissions;
    }

    public async Task<bool> AuthorizedAsync(string appId, string code, Guid userId)
    {
        var query = new UserAuthorizedDomainEvent(appId, code, userId);
        await EventBus.PublishAsync(query);
        return query.Authorized;
    }
}

