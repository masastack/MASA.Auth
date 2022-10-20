// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Contrib.Authentication.OpenIdConnect.Cache.Utils;

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class StaffCacheCommandHandler
{
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IStaffRepository _staffRepository;

    public StaffCacheCommandHandler(IMultilevelCacheClient multilevelCacheClient, IStaffRepository staffRepository)
    {
        _multilevelCacheClient = multilevelCacheClient;
        _staffRepository = staffRepository;
    }

    [EventHandler(99)]
    public async Task AddStaffAsync(AddStaffCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task UpdateStaffAsync(UpdateStaffCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task UpsertStaffAsync(UpsertStaffCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task UpsertStaffForLdapAsync(UpsertStaffForLdapCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task UpdateStaffAvatarAsync(UpdateStaffAvatarCommand command)
    {
        await SetStaffCacheAsync(command.Result);
    }

    [EventHandler(99)]
    public async Task RemoveStaffAsync(RemoveStaffCommand command)
    {
        var staffs = await _multilevelCacheClient.GetAsync<List<Staff>>(FormatKey()) ?? new();
        staffs.Remove(s => s.Id == command.Staff.Id);
        await _multilevelCacheClient.SetAsync(FormatKey(), staffs);
    }

    [EventHandler(99)]
    public async Task SyncAsync(SyncStaffCommand command)
    {
        var staffs = await _staffRepository.GetListAsync();
        await _multilevelCacheClient.SetAsync(FormatKey(), staffs);
    }

    [EventHandler(1)]
    public async Task UpdateStaffDefaultPasswordAsync(UpdateStaffDefaultPasswordCommand command)
    {
        await _multilevelCacheClient.SetAsync(CacheKey.STAFF, command.DefaultPassword);
    }
    
    async Task SetStaffCacheAsync(Staff? staff)
    {
        if (staff is null) return;
        var staffs = await _multilevelCacheClient.GetAsync<List<Staff>>(FormatKey()) ?? new();
        staffs.Set(staff, s => s.Id == staff.Id);
        await _multilevelCacheClient.SetAsync(FormatKey(), staffs);
    }

    string FormatKey()
    {
        return "all_staff";
    }
}
