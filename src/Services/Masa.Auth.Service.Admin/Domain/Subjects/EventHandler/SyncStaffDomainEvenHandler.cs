// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class SyncStaffDomainEvenHandler
{
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;
    readonly IPositionRepository _positionRepository;
    readonly IEventBus _eventBus;
    readonly UserDomainService _userDomainService;

    public SyncStaffDomainEvenHandler(IUserRepository userRepository, IStaffRepository staffRepository, IPositionRepository positionRepository, IEventBus eventBus, UserDomainService userDomainService)
    {
        _userRepository = userRepository;
        _staffRepository = staffRepository;
        _positionRepository = positionRepository;
        _eventBus = eventBus;
        _userDomainService = userDomainService;
    }

    [EventHandler(1)]
    public async Task SyncAsync(SyncStaffDomainEvent staffEvent)
    {
        var syncResults = new SyncStaffResultsDto();
        staffEvent.Result = syncResults;
        var syncStaffs = staffEvent.Staffs;
        //validation
        var validator = new SyncStaffValidator();
        foreach (var staff in syncStaffs)
        {
            var result = validator.Validate(staff);
            if (result.IsValid is false)
            {
                syncResults[staff.Index] = new()
                {
                    Account = staff.Account,
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        CheckDuplicate(Staff => Staff.Account);
        CheckDuplicate(Staff => Staff.PhoneNumber);
        CheckDuplicate(Staff => Staff.JobNumber);
        CheckDuplicate(Staff => Staff.Email);
        CheckDuplicate(Staff => Staff.IdCard);
        if (syncResults.IsValid) return;

        //sync user
        var query = new AllUsersQuery();
        await _eventBus.PublishAsync(query);
        var allUsers = query.Result;
        var userRange = new List<User>();
        foreach (var staff in syncStaffs)
        {
            var existUsers = allUsers.Where(u => u.PhoneNumber == staff.PhoneNumber || u.Email == staff.Email || u.IdCard == staff.IdCard);
            var oldUser = allUsers.FirstOrDefault(s => s.Account == staff.Account);
            if (existUsers.Count(u => u.Id != oldUser?.Id) > 0)
            {
                syncResults[staff.Index] = new()
                {
                    Account = staff.Account,
                    Errors = new() { $"Users with the same mobile phone number, ID number, and email address already exist" }
                };
            }
            else
            {
                if (oldUser is null) oldUser = new User(staff.Name, staff.DisplayName ?? "", "", staff.IdCard ?? "", staff.Account, staff.Password, "", "", staff.Position ?? "", true, staff.PhoneNumber ?? "", staff.Email ?? "", staff.GenderType);
                else oldUser.Update(staff.Name, staff.DisplayName, staff.IdCard, staff.PhoneNumber, staff.Email, staff.Position, staff.Password, staff.GenderType);

                userRange.Add(oldUser);
            }
        }
        if (syncResults.IsValid) return;
        var updateUserRange = userRange.Where(u => u.Id != default).ToList();
        if (updateUserRange.Count > 0) await _userRepository.UpdateRangeAsync(updateUserRange);
        var addUserRange = userRange.Where(u => u.Id == default).ToList();
        if (addUserRange.Count > 0) await _userRepository.AddRangeAsync(addUserRange);
        await _userDomainService.SetAsync(userRange.ToArray());

        //sync psoition
        var syncPsoitions = syncStaffs.Select(staff => staff.Position)
                                      .Distinct()
                                      .Where(position => position is not null)
                                      .Select(position => new Position(position!));
        var allPositions = (await _positionRepository.GetListAsync()).ToList();
        var addPositionRange = syncPsoitions.Where(sp => allPositions.Any(p => p.Name == sp.Name) is false).ToList();
        await _positionRepository.AddRangeAsync(addPositionRange);
        allPositions.AddRange(addPositionRange);

        //sync staff
        var allStaffs = await _staffRepository.GetListAsync();
        var staffRange = new List<Staff>();
        foreach (var staff in syncStaffs)
        {
            var user = userRange.First(u => u.Account == staff.Account);
            var position = allPositions.FirstOrDefault(p => p.Name == staff.Position);
            var oldStaff = allStaffs.FirstOrDefault(s => s.JobNumber == staff.JobNumber);
            if (oldStaff is null)
            {
                staffRange.Add(new Staff(user.Id, staff.JobNumber, staff.Name, position?.Id, staff.StaffType, true));
            }
            else
            {
                oldStaff.Update(staff.Name, position?.Id, staff.StaffType, oldStaff.Enabled);
                staffRange.Add(oldStaff);
            }
        }
        if (syncResults.IsValid) return;

        var updateStaffRange = staffRange.Where(s => s.Id != default).ToList();
        if (updateStaffRange.Count > 0) await _staffRepository.UpdateRangeAsync(updateStaffRange);
        var addStaffRange = staffRange.Where(s => s.Id == default).ToList();
        if (addStaffRange.Count > 0) await _staffRepository.AddRangeAsync(addStaffRange);

        void CheckDuplicate(Expression<Func<SyncStaffDto, string?>> selector)
        {
            var func = selector.Compile();
            if (syncStaffs.Where(staff => func(staff) is not null).IsDuplicate(func, out List<SyncStaffDto>? duplicate))
            {
                foreach (var staff in duplicate)
                {
                    syncResults[staff.Index] = new()
                    {
                        Account = staff.Account,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }
    }
}
