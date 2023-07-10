// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class OperaterProvider : IScopedDependency
{
    IMultilevelCacheClient _cacheClient;
    IStaffRepository _staffRepository;

    public OperaterProvider(IMultilevelCacheClient cacheClient, IStaffRepository staffRepository)
    {
        _cacheClient = cacheClient;
        _staffRepository = staffRepository;
    }

    public async Task<(string? creator, string? modifier)> GetActionInfoAsync(Guid creator, Guid modifier)
    {
        string? creatorName = "", modifierName = "";
        if (creator != default)
        {
            creatorName = (await _staffRepository.FindAsync(creator).ConfigureAwait(false))?.DisplayName;
            if (creatorName is null)
                creatorName = (await _cacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(creator)).ConfigureAwait(false))?.DisplayName;
        }
        if (modifier != default)
        {
            modifierName = (await _staffRepository.FindAsync(modifier).ConfigureAwait(false))?.DisplayName;
            if (modifierName is null)
                modifierName = (await _cacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(modifier)).ConfigureAwait(false))?.DisplayName;
        }

        return (creatorName, modifierName);
    }

    public async Task<UserModel?> GetUserAsync(Guid userId)
    {
        return await _cacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
    }
}
