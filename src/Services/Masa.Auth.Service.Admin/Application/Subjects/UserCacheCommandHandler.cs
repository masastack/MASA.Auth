// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class UserCacheCommandHandler
{
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly UserDomainService _userDomainService;

    public UserCacheCommandHandler(
        IMultilevelCacheClient multilevelCacheClient,
        UserDomainService userDomainService)
    {
        _multilevelCacheClient = multilevelCacheClient;
        _userDomainService = userDomainService;
    }

    [EventHandler(99)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand updateUserAuthorizationCommand)
    {
        await _userDomainService.SyncUserAsync(updateUserAuthorizationCommand.User.Id);
    }

    [EventHandler]
    public async Task SyncUserRedisAsync(SyncUserRedisCommand command)
    {
        await _userDomainService.SyncUsersAsync();
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

    [EventHandler(99)]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        await _userDomainService.SyncUserAsync(command.User.Id);
    }

    [EventHandler(99)]
    public async Task UpsertUserAsync(UpsertUserCommand command)
    {
        await _userDomainService.SyncUserAsync(command.Result.Id);
    }
}
