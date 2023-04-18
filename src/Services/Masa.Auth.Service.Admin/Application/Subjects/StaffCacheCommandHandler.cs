// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
    public async Task ChangeStaffCurrentTeamAsync(UpdateStaffCurrentTeamCommand updateStaffCurrentTeamCommand)
    {
        await SetStaffCacheAsync(updateStaffCurrentTeamCommand.Result);
    }

    [EventHandler(99)]
    public async Task UpdateStaffBasicInfoAsync(UpdateStaffBasicInfoCommand command)
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
        if (command.Result is not null)
            await _multilevelCacheClient.RemoveAsync<CacheStaff>(CacheKey.StaffKey(command.Result.Id));
    }

    [EventHandler(99)]
    public async Task SyncStaffAsync(SyncStaffCommand command)
    {
        await SyncStaffCacheAsync(new());
    }

    [EventHandler(1)]
    public async Task SyncStaffCacheAsync(SyncStaffCacheCommand command)
    {
        var staffs = await _staffRepository.GetListAsync() ?? new List<Staff>();
        await _multilevelCacheClient.SetListAsync(staffs.ToDictionary(staff => CacheKey.StaffKey(staff.UserId), staff => (CacheStaff)staff));
    }

    [EventHandler(1)]
    public async Task UpdateStaffDefaultPasswordAsync(UpdateStaffDefaultPasswordCommand command)
    {
        await _multilevelCacheClient.SetAsync(CacheKey.STAFF_DEFAULT_PASSWORD, command.DefaultPassword);
    }

    async Task SetStaffCacheAsync(Staff? staff)
    {
        if (staff is null) return;
        await _multilevelCacheClient.SetAsync(CacheKey.StaffKey(staff.UserId), (CacheStaff)staff);
    }
}
