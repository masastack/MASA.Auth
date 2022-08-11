// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class UserDomainService : DomainService
{
    readonly AuthDbContext _authDbContext;

    public UserDomainService(IDomainEventBus eventBus, AuthDbContext authDbContext) : base(eventBus)
    {
        _authDbContext = authDbContext;
    }

    public async Task SetAsync(params User[] users)
    {
        await EventBus.PublishAsync(new SetUserDomainEvent(users.ToList()));
    }

    public async Task RemoveAsync(params Guid[] userIds)
    {
        await EventBus.PublishAsync(new RemoveUserDomainEvent(userIds.ToList()));
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

