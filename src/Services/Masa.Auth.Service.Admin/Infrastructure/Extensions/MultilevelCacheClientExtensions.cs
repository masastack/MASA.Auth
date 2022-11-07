// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Extensions;

public static class MultilevelCacheClientExtensions
{
    public static async Task<(string? creator, string? modifier)> GetActionInfoAsync(this IMultilevelCacheClient client, Guid creator, Guid modifier)
    {
        var creatorName = (await client.GetAsync<CacheStaff>(CacheKey.StaffKey(creator)).ConfigureAwait(false))?.DisplayName;
        var modifierName = (await client.GetAsync<CacheStaff>(CacheKey.StaffKey(modifier)).ConfigureAwait(false))?.DisplayName;
        if (creatorName is null)
            creatorName = (await client.GetAsync<UserModel>(CacheKeyConsts.UserKey(creator)).ConfigureAwait(false))?.DisplayName;
        if (modifierName is null)
            modifierName = (await client.GetAsync<UserModel>(CacheKeyConsts.UserKey(modifier)).ConfigureAwait(false))?.DisplayName;

        return (creatorName, modifierName);
    }

    public static async Task<UserModel?> GetUserAsync(this IMultilevelCacheClient client, Guid userId)
    {
       return await client.GetAsync<UserModel>(CacheKeyConsts.UserKey(userId));
    }
}
