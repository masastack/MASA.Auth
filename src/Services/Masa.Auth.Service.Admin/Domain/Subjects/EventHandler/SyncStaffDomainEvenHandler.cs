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
        if (syncResults.IsValid is false) return;

        //sync user
        var query = new AllUsersQuery();
        await _eventBus.PublishAsync(query);
        var allUsers = query.Result;
        var userRange = new List<User>();
        foreach (var staff in syncStaffs)
        {
            var oldUser = allUsers.FirstOrDefault(s => s.Account == staff.Account);
            if (oldUser is null) userRange.Add(new User(staff.Name, staff.DisplayName ?? "", "", staff.IdCard ?? "", staff.Account, "123", "", "", staff.Position ?? "", true, staff.PhoneNumber ?? "", staff.Email ?? "", staff.GenderType));
            else
            {
                if (allUsers.Any(u => u.Id != oldUser.Id && (u.PhoneNumber == staff.PhoneNumber || u.Email == staff.Email || u.IdCard == staff.IdCard)))
                {
                    syncResults[staff.Index] = new ()
                    {
                        Account = staff.Account,
                        Errors = new() { $"Users with the same mobile phone number, ID number, and email address already exist" }
                    };
                }
                oldUser.Update(staff.Name, staff.DisplayName, staff.IdCard, staff.PhoneNumber, staff.Email, staff.Position, staff.GenderType);
            }
        }
        if (syncResults.IsValid is false) return;
        await _userRepository.AddRangeAsync(userRange.Where(u => u.Id == default));
        await _userRepository.UpdateRangeAsync(userRange.Where(u => u.Id != default));
        await _userDomainService.SetAsync(userRange.ToArray());

        //sync psoition
        var syncPsoitions = syncStaffs.Select(staff => staff.Position)
                                      .Distinct()
                                      .Where(position => position is not null)
                                      .Select(position => new Position(position!));
        var allPositions = await _positionRepository.GetListAsync();
        await _positionRepository.AddRangeAsync(syncPsoitions.Where(sp => allPositions.Any(p => p.Name == sp.Name) is false));
        allPositions = await _positionRepository.GetListAsync();

        //sync staff
        var allStaffs = await _staffRepository.GetListAsync();
        var staffRange = new List<Staff>();
        foreach (var staff in syncStaffs)
        {
            var user = userRange.First(u => u.Account == staff.Account);
            var position = allPositions.FirstOrDefault(p => p.Name == staff.Name);
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
        await _staffRepository.AddRangeAsync(staffRange.Where(s => s.Id == default));
        await _staffRepository.UpdateRangeAsync(staffRange.Where(s => s.Id != default));

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
