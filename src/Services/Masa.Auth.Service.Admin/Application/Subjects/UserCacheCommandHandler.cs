// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class UserCacheCommandHandler
{
    readonly IDistributedCacheClient _cacheClient;
    readonly UserDomainService _userDomainService;
    readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public UserCacheCommandHandler(
        IDistributedCacheClient cacheClient,
        UserDomainService userDomainService,
        IMultiEnvironmentContext multiEnvironmentContext)
    {
        _cacheClient = cacheClient;
        _userDomainService = userDomainService;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler(99)]
    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationCommand updateUserAuthorizationCommand)
    {
        await BackgroundJobManager.EnqueueAsync(new SyncUserArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            UserId = updateUserAuthorizationCommand.User.Id
        });
    }

    [EventHandler]
    public async Task SyncUserRedisAsync(SyncUserRedisCommand command)
    {
        var args = new SyncUserRedisArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
        };

        await BackgroundJobManager.EnqueueAsync(args);
    }

    [EventHandler]
    public async Task UserVisitedAsync(UserVisitedCommand userVisitedCommand)
    {
        var dto = userVisitedCommand.AddUserVisitedDto;
        //todo zset
        var key = CacheKey.UserVisitKey(dto.UserId);
        var visited = await _cacheClient.GetOrSetAsync(key, () =>
        {
            return new CacheEntry<List<CacheUserVisited>>(new List<CacheUserVisited>());
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
        await _cacheClient.SetAsync(key, visited);
    }

    [EventHandler(99)]
    public async Task SaveUserSystemBusinessDataAsync(SaveUserSystemBusinessDataCommand saveUserSystemBusinessDataCommand)
    {
        var userSystemData = saveUserSystemBusinessDataCommand.UserSystemData;
        await _cacheClient.SetAsync(
            CacheKey.UserSystemDataKey(userSystemData.UserId, userSystemData.SystemId),
            userSystemData.Data);
    }

    [EventHandler(99)]
    public async Task UpdateUserBasicInfoAsync(UpdateUserBasicInfoCommand command)
    {
        await BackgroundJobManager.EnqueueAsync(new SyncUserArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            UserId = command.User.Id
        });
    }

    [EventHandler(99)]
    public async Task UpsertUserAsync(UpsertUserCommand command)
    {
        await BackgroundJobManager.EnqueueAsync(new SyncUserArgs()
        {
            Environment = _multiEnvironmentContext.CurrentEnvironment,
            UserId = command.Result.Id
        });
    }
}
