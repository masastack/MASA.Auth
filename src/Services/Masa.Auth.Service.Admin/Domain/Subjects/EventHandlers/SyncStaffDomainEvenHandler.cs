// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

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
                    //Account = staff.Account,
                    Errors = result.Errors.Select(e => e.ErrorMessage).ToList()
                };
            }
        }
        //check duplicate
        //CheckDuplicate(Staff => Staff.Account);
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
        //foreach (var staff in syncStaffs)
        //{
        //    var existUsers = allUsers.Where(u =>
        //        string.IsNullOrEmpty(u.PhoneNumber) is false && u.PhoneNumber == staff.PhoneNumber ||
        //        string.IsNullOrEmpty(u.Email) is false && u.Email == staff.Email ||
        //        string.IsNullOrEmpty(u.IdCard) is false && u.IdCard == staff.IdCard);
        //    var oldUser = allUsers.FirstOrDefault(s => s.Account == staff.Account);
        //    if (existUsers.Count(u => u.Id != oldUser?.Id) > 0)
        //    {
        //        syncResults[staff.Index] = new()
        //        {
        //            Account = staff.Account,
        //            Errors = new() { $"Users with the same mobile phone number, ID number, and email address already exist" }
        //        };
        //    }
        //    else
        //    {
        //        if (oldUser is null)
        //        {
        //            userRange.Add(new User(
        //                staff.Name, staff.DisplayName ?? "", DefaultUserAttributes.GetDefaultAvatar(staff.Gender), staff.IdCard ?? "",
        //                staff.Account, staff.Password, "", "", staff.Position ?? "", true,
        //                staff.PhoneNumber ?? "", "", staff.Email ?? "", staff.Gender)
        //           );
        //        }
        //    }
        //}
        if (syncResults.IsValid) return;
        if (userRange.Count > 0)
        {
            await _userRepository.AddRangeAsync(userRange);
            await _userDomainService.SetAsync(userRange.ToArray());
            allUsers.AddRange(userRange);
        }

        //sync psoition
        var syncPsoitions = syncStaffs.Select(staff => staff.Position)
                                      .Distinct()
                                      .Where(position => position is not null)
                                      .Select(position => new Position(position!));
        var allPositions = (await _positionRepository.GetListAsync()).ToList();
        var addPositionRange = syncPsoitions.Where(sp => allPositions.Any(p => p.Name == sp.Name) is false).ToList();
        if (addPositionRange.Count > 0)
        {
            await _positionRepository.AddRangeAsync(addPositionRange);
            allPositions.AddRange(addPositionRange);
        }

        //sync staff
        var allStaffs = await _staffRepository.GetListAsync();
        var staffRange = new List<Staff>();
        //foreach (var staff in syncStaffs)
        //{
        //    var user = allUsers.First(u => u.Account == staff.Account);
        //    var position = allPositions.FirstOrDefault(p => p.Name == staff.Position);
        //    var oldStaff = allStaffs.FirstOrDefault(s => s.JobNumber == staff.JobNumber);
        //    var existStaffs = allStaffs.Where(s =>
        //        string.IsNullOrEmpty(s.JobNumber) is false && s.JobNumber == staff.JobNumber ||
        //        string.IsNullOrEmpty(s.PhoneNumber) is false && s.PhoneNumber == staff.PhoneNumber ||
        //        string.IsNullOrEmpty(s.Email) is false && s.Email == staff.Email ||
        //        string.IsNullOrEmpty(s.IdCard) is false && s.IdCard == staff.IdCard);
        //    if (existStaffs.Count(s => s.Id != oldStaff?.Id) > 0)
        //    {
        //        syncResults[staff.Index] = new()
        //        {
        //            Account = staff.Account,
        //            Errors = new() { $"Staffs with the same mobile phone number, ID number, JobNumber, and email address already exist" }
        //        };
        //    }
        //    else
        //    {
        //        if (oldStaff is null)
        //        {
        //            staffRange.Add(new Staff(
        //                user.Id, 
        //                staff.Name, 
        //                staff.DisplayName, 
        //                user.Avatar,
        //                staff.IdCard, 
        //                user.CompanyName,
        //                staff.Gender, 
        //                staff.PhoneNumber, 
        //                staff.Email,
        //                staff.JobNumber, 
        //                position?.Id, 
        //                staff.StaffType, 
        //                true, 
        //                user.Address));
        //        }
        //        else
        //        {
        //            oldStaff.Update(
        //                position?.Id, staff.StaffType, oldStaff.Enabled, staff.Name,
        //                staff.DisplayName, oldStaff.Avatar, staff.IdCard, oldStaff.CompanyName,
        //                staff.PhoneNumber, staff.Email, oldStaff.Address, staff.Gender);
        //            staffRange.Add(oldStaff);
        //        }
        //    }
        //}
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
                        //Account = staff.Account,
                        Errors = new() { $"{(selector.Body as MemberExpression)!.Member.Name}:{func(staff)} - duplicate" }
                    };
                }
            }
        }
    }
}
