// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class UserCacheCommandHandler
{
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IUserRepository _userRepository;

    public UserCacheCommandHandler(IMultilevelCacheClient multilevelCacheClient, IUserRepository userRepository)
    {
        _multilevelCacheClient = multilevelCacheClient;
        _userRepository = userRepository;
    }

    async Task SetUserListCacheAsync(IEnumerable<User> users)
    {
        var map = users.ToDictionary(u => CacheKey.UserKey(u.Id), u => u.Adapt<CacheUser>());
        await _multilevelCacheClient.SetListAsync(map);
    }

    async Task SetUserCacheAsync(Guid userId)
    {
        var user = await _userRepository.GetDetailAsync(userId);
        if (user is not null)
        {
            await _multilevelCacheClient.SetAsync(CacheKey.UserKey(userId), user.Adapt<CacheUser>());
        }
    }

    async Task RemoveUserCahceAsync(Guid userId)
    {
        await _multilevelCacheClient.RemoveAsync<CacheUser>(CacheKey.UserKey(userId));
    }

    [EventHandler(99)]
    public async Task AddUserAsync(AddUserCommand addUserCommand)
    {
        await SetUserCacheAsync(addUserCommand.Result.Id);
    }

    [EventHandler(99, IsCancel = true)]
    public async Task FailAddUserAsync(AddUserCommand addUserCommand)
    {
        await RemoveUserCahceAsync(addUserCommand.Result.Id);
    }

    [EventHandler(99)]
    public async Task UpdateUserAsync(UpdateUserCommand updateUserCommand)
    {
        await SetUserCacheAsync(updateUserCommand.User.Id);
    }

    [EventHandler(99)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand updateUserAuthorizationCommand)
    {
        await SetUserCacheAsync(updateUserAuthorizationCommand.User.Id);
    }

    [EventHandler(99)]
    public async Task RemoveUserAsync(RemoveUserCommand removeUserCommand)
    {
        await RemoveUserCahceAsync(removeUserCommand.User.Id);
    }

    [EventHandler]
    public async Task SyncUserRedisAsync(SyncUserRedisCommand command)
    {
        var users = await _userRepository.GetAllAsync();
        var syncCount = 0;
        while (syncCount < users.Count)
        {
            var syncUsers = users.Skip(syncCount)
                                .Take(command.Dto.OnceExecuteCount);
            await SetUserListCacheAsync(syncUsers);
            syncCount += command.Dto.OnceExecuteCount;
        }
    }

    [EventHandler]
    public async Task UserVisitedAsync(UserVisitedCommand userVisitedCommand)
    {
        var dto = userVisitedCommand.AddUserVisitedDto;
        //todo zset
        var key = CacheKey.UserVisitKey(dto.UserId);
        var visited = await _multilevelCacheClient.GetOrSetAsync(key, new CombinedCacheEntry<List<CacheUserVisited>>
        {
            DistributedCacheEntryFunc = () =>
            {
                return new CacheEntry<List<CacheUserVisited>>(new List<CacheUserVisited>());
            }
        });
        visited ??= new List<CacheUserVisited>();
        var item = new CacheUserVisited
        {
            AppId = dto.AppId,
            Url = dto.Url.Trim('/').ToLower()
        };
        visited.RemoveAll(v => v.AppId == item.AppId && v.Url == item.Url);
        visited.Insert(0, item);
        if (visited.Count > 10)
        {
            visited = visited.GetRange(0, 10);
        }
        await _multilevelCacheClient.SetAsync(key, visited);
    }

    [EventHandler(99)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand saveUserSystemBusinessDataCommand)
    {
        var userSystemData = saveUserSystemBusinessDataCommand.UserSystemData;
        await _multilevelCacheClient.SetAsync(
            CacheKey.UserSystemDataKey(userSystemData.UserId, userSystemData.SystemId),
            userSystemData.Data);
    }
}
