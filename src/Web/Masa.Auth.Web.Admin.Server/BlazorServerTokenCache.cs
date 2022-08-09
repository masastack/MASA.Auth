// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin;

public class BlazorServerTokenCache
{
    IMemoryCache _memoryCache = new MemoryCache(new MemoryCacheOptions());

    public void Add(string subjectId, BlazorServerTokenData data)
    {
        Debug.WriteLine($"Caching sid: {subjectId}");
        _memoryCache.Set(subjectId, data, new MemoryCacheEntryOptions()
        {
            SlidingExpiration = TimeSpan.FromDays(5)
        });
    }

    public BlazorServerTokenData? Get(string subjectId)
    {
        _memoryCache.TryGetValue(subjectId, out BlazorServerTokenData data);
        return data;
    }

    public void Remove(string subjectId)
    {
        Debug.WriteLine($"Removing sid: {subjectId}");
        _memoryCache.Remove(subjectId);
    }
}
