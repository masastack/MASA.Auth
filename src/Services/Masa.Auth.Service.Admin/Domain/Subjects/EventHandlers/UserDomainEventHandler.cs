// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class UserDomainEventHandler
{
    readonly AuthDbContext _authDbContext;
    readonly IEventBus _eventBus;
    readonly IDistributedCacheClient _cacheClient;

    public UserDomainEventHandler(
        AuthDbContext authDbContext,
        IEventBus eventBus,
        IDistributedCacheClient cacheClient)
    {
        _authDbContext = authDbContext;
        _eventBus = eventBus;
        _cacheClient = cacheClient;
    }

    [EventHandler(1)]
    public async Task GetPermissions(QueryUserPermissionDomainEvent userEvent)
    {
        var user = await GetUserAsync(userEvent.UserId);
        if (user.Account == "admin")
        {
            var cachePermissions = await _cacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
            if (cachePermissions?.Count > 0)
            {
                userEvent.Permissions = cachePermissions.Select(e => e!.Id).ToList();
            }
            else
            {
                userEvent.Permissions = await _authDbContext.Set<Permission>()
                    .Select(p => p.Id).ToListAsync();
            }
        }
        else
        {
            var query = new PermissionsByUserQuery(userEvent.UserId, userEvent.Teams);
            await _eventBus.PublishAsync(query);
            userEvent.Permissions = query.Result;
        }
    }

    private async Task<UserModel> GetUserAsync(Guid userId)
    {
        var userModel = await _cacheClient.GetAsync<UserModel>(CacheKey.UserKey(userId));
        if (userModel == null)
        {
            var user = await _authDbContext.Set<User>()
                                       .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.USER_NOT_EXIST);
            }
            userModel = user.Adapt<UserModel>();
        }
        return userModel;
    }
}
