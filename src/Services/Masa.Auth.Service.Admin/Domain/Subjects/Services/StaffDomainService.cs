// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

public class StaffDomainService : DomainService
{
    readonly UserDomainService _userDomainService;

    public StaffDomainService(UserDomainService userDomainService)
    {
        _userDomainService = userDomainService;
    }

    public async Task RemoveAsync(RemoveStaffDto staff)
    {
        await EventBus.PublishAsync(new RemoveStaffDomainEvent(staff));
    }

    public async Task<Staff> AddAsync(AddStaffDto staffDto)
    {
        var staff = new Staff(
                staffDto.Name,
                staffDto.DisplayName,
                staffDto.Avatar,
                staffDto.IdCard,
                staffDto.CompanyName,
                staffDto.Gender,
                staffDto.PhoneNumber,
                staffDto.Email,
                staffDto.JobNumber,
                null,
                staffDto.StaffType,
                staffDto.Enabled,
                staffDto.Address
            );
        await _userDomainService.AddAsync(new User(staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.Account,
            staffDto.Password, staffDto.CompanyName, staffDto.Email, staffDto.PhoneNumber, staff));
        return staff;
    }
}
