// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure;

public class OperaterProvider : IScopedDependency
{
    IDistributedCacheClient _cacheClient;
    IStaffRepository _staffRepository;

    public OperaterProvider(IDistributedCacheClient cacheClient, IStaffRepository staffRepository)
    {
        _cacheClient = cacheClient;
        _staffRepository = staffRepository;
    }

    public async Task<(string? creator, string? modifier)> GetActionInfoAsync(Guid creator, Guid modifier)
    {
        string? creatorName = "", modifierName = "";
        if (creator != default)
        {
            creatorName = await GetDisplayNameAsync(creator).ConfigureAwait(false);
        }
        if (modifier != default)
        {
            modifierName = await GetDisplayNameAsync(modifier).ConfigureAwait(false);
        }

        return (creatorName, modifierName);
    }

    /// <summary>
    /// 缓存优先取操作人显示名，未命中再按 UserId 兜底查 Staff。
    /// Creator/Modifier 存的是 User Id（SaveChangeFilter 写入 _userContext.UserId），
    /// 不能按 Staff 主键 Id 查——Staff.Id 与 Staff.UserId 是两个键（历史 bug：旧实现按主键查，几乎必空）。
    /// 多级缓存自带进程内 L1，同一请求的重复读取为内存命中。
    /// </summary>
    private async Task<string?> GetDisplayNameAsync(Guid userId)
    {
        var userModel = await _cacheClient.GetAsync<UserModel>(CacheKey.UserKey(userId)).ConfigureAwait(false);
        if (userModel?.DisplayName is { Length: > 0 } displayName)
        {
            return displayName;
        }

        return (await _staffRepository.FindAsync(staff => staff.UserId == userId).ConfigureAwait(false))?.DisplayName;
    }

    public async Task<UserModel?> GetUserAsync(Guid userId)
    {
        return await _cacheClient.GetAsync<UserModel>(CacheKey.UserKey(userId));
    }
}
