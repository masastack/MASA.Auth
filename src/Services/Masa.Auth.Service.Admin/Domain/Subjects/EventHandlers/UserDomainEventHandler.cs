// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class UserDomainEventHandler
{
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly IEventBus _eventBus;
    readonly IMultilevelCacheClient _multilevelCacheClient;

    public UserDomainEventHandler(
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService,
        IEventBus eventBus,
        IMultilevelCacheClient multilevelCacheClient)
    {
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
        _eventBus = eventBus;
        _multilevelCacheClient = multilevelCacheClient;
    }

    [EventHandler(1)]
    public async Task UpdateRoleLimitAsync(UpdateUserAuthorizationDomainEvent userEvent)
    {
        await _roleDomainService.UpdateRoleLimitAsync(userEvent.Roles);
    }

    [EventHandler(1)]
    public async Task GetPermissions(QueryUserPermissionDomainEvent userEvent)
    {
        var user = await GetUserAsync(userEvent.UserId);
        if (user.Account == "admin")
        {
            var cachePermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
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
        var userModel = await _multilevelCacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
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
