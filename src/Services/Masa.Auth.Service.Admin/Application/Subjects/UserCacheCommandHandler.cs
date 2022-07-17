// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class UserCacheCommandHandler
{
    readonly IMemoryCacheClient _memoryCacheClient;

    public UserCacheCommandHandler(IMemoryCacheClient memoryCacheClient)
    {
        _memoryCacheClient = memoryCacheClient;
    }

    [EventHandler(99)]
    public async Task AddUserAsync(AddUserCommand addUserCommand)
    {
        await _memoryCacheClient.SetAsync($"{CacheKey.USER_CACHE_KEY_PRE}{addUserCommand.NewUser.Id}", addUserCommand.User.Adapt<CacheUser>());
    }

    [EventHandler(99, IsCancel = true)]
    public async Task FailAddUserAsync(AddUserCommand addUserCommand)
    {
        await _memoryCacheClient.RemoveAsync<CacheUser>($"{CacheKey.USER_CACHE_KEY_PRE}{addUserCommand.NewUser.Id}");
    }

    [EventHandler(99)]
    public async Task UpdateUserAsync(UpdateUserCommand updateUserCommand)
    {
        var key = $"{CacheKey.USER_CACHE_KEY_PRE}{updateUserCommand.User.Id}";
        var cacheUser = updateUserCommand.User.Adapt<CacheUser>();
        var oldCache = _memoryCacheClient.Get<CacheUser>(key);
        if (oldCache != null)
        {
            cacheUser.Roles = oldCache.Roles;
            cacheUser.Permissions = oldCache.Permissions;
        }
        await _memoryCacheClient.SetAsync(key, cacheUser);
    }

    [EventHandler(99)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand updateUserAuthorizationCommand)
    {
        var key = $"{CacheKey.USER_CACHE_KEY_PRE}{updateUserAuthorizationCommand.User.Id}";
        var oldCache = _memoryCacheClient.Get<CacheUser>(key);
        if (oldCache != null)
        {
            oldCache.Roles = updateUserAuthorizationCommand.User.Roles;
            oldCache.Permissions = updateUserAuthorizationCommand.User.Permissions;
            await _memoryCacheClient.SetAsync(key, oldCache);
        }
    }

    [EventHandler(99)]
    public async Task RemoveUserAsync(RemoveUserCommand removeUserCommand)
    {
        await _memoryCacheClient.RemoveAsync<CacheUser>($"{CacheKey.USER_CACHE_KEY_PRE}{removeUserCommand.User.Id}");
    }

    [EventHandler]
    public async Task UserVisitedAsync(UserVisitedCommand userVisitedCommand)
    {
        //todo zset
        var key = $"{CacheKey.USER_VISIT_PRE}{userVisitedCommand.UserId}";
        var visited = await _memoryCacheClient.GetOrSetAsync<List<string>>(key, () =>
        {
            return new List<string>();
        });
        visited?.Remove(userVisitedCommand.Url);
        visited?.Insert(0, userVisitedCommand.Url);
        if (visited?.Count > 5)
        {
            visited = visited.GetRange(0, 5);
        }
        await _memoryCacheClient.SetAsync(key, visited);
    }

    [EventHandler(99)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand saveUserSystemBusinessDataCommand)
    {
        await _memoryCacheClient.SetAsync(
            CacheKey.UserSystemDataKey(saveUserSystemBusinessDataCommand.UserId, saveUserSystemBusinessDataCommand.SystemId),
            saveUserSystemBusinessDataCommand.Data);
    }
}
