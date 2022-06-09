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
        await _memoryCacheClient.SetAsync($"{CacheKey.USER_CACHE_KEY_PRE}{updateUserCommand.User.Id}", updateUserCommand.User.Adapt<CacheUser>());
    }

    [EventHandler(99)]
    public async Task RemoveUserAsync(RemoveUserCommand removeUserCommand)
    {
        await _memoryCacheClient.RemoveAsync<CacheUser>($"{CacheKey.USER_CACHE_KEY_PRE}{removeUserCommand.User.Id}");
    }
}
