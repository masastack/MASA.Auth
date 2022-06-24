// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class UpdateStaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IEventBus _eventBus;

    public UpdateStaffDomainEventHandler(IStaffRepository staffRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _eventBus = eventBus;
    }

    //[EventHandler(1)]
    //public async Task UpdateUserAsync(UpdateStaffDomainEvent staffEvent)
    //{
    //    var command = new UpdateUserAuthorizationCommand(staffEvent.Staff.User);
    //    await _eventBus.PublishAsync(command);
    //}

    [EventHandler(2)]
    public async Task AddPositionAsync(UpdateStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position) is false)
        {
            var command = new AddPositionCommand(new(staffEvent.Staff.Position));
            await _eventBus.PublishAsync(command);
            staffEvent.Staff.PositionId = command.Result;
        }
    }

    [EventHandler(3)]
    public async Task UpdateStaffAsync(UpdateStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var staff = await _staffRepository.FindAsync(s => s.Id == staffDto.Id);
        if (staff is null)
            throw new UserFriendlyException("This staff data does not exist");

        Expression<Func<Staff, bool>> condition = staff => staff.JobNumber == staffDto.JobNumber;
        if (!string.IsNullOrEmpty(staffDto.PhoneNumber))
            condition = condition.Or(staff => staff.PhoneNumber == staffDto.PhoneNumber);
        if (!string.IsNullOrEmpty(staffDto.Email))
            condition = condition.Or(staff => staff.Email == staffDto.Email);
        if (!string.IsNullOrEmpty(staffDto.IdCard))
            condition = condition.Or(staff => staff.IdCard == staffDto.IdCard);

        Expression<Func<Staff, bool>> condition2 = staff => staff.Id != staffDto.Id;
        var existStaff = await _staffRepository.FindAsync(condition2.And(condition));
        if (existStaff is not null)
        {
            if (string.IsNullOrEmpty(staffDto.PhoneNumber) is false && staffDto.PhoneNumber == existStaff.PhoneNumber)
                throw new UserFriendlyException($"Staff with phone number {staffDto.PhoneNumber} already exists");                      
            if (string.IsNullOrEmpty(staffDto.Email) is false && staffDto.Email == existStaff.Email)
                throw new UserFriendlyException($"Staff with email {staffDto.Email} already exists");
            if (string.IsNullOrEmpty(staffDto.IdCard) is false && staffDto.IdCard == existStaff.IdCard)
                throw new UserFriendlyException($"Staff with idCard {staffDto.IdCard} already exists");
            if (string.IsNullOrEmpty(staffDto.JobNumber) is false && staffDto.JobNumber == existStaff.JobNumber)
                throw new UserFriendlyException($"Staff with jobNumber number {staffDto.JobNumber} already exists");
        }

        staff.Update(
            staffDto.PositionId, staffDto.StaffType, staffDto.Enabled, staffDto.Name,
            staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName, 
            staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Gender);
        staff.AddDepartmentStaff(staffDto.DepartmentId);
        staff.AddTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);
    }
}
