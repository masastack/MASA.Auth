// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class AddStaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IEventBus _eventBus;

    public AddStaffDomainEventHandler(IStaffRepository staffRepository, IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddStaffDomainEvent staffEvent)
    {
        var command = new AddUserCommand(staffEvent.Staff.User);
        await _eventBus.PublishAsync(command);
        staffEvent.Staff.UserId = command.NewUser.Id;
    }

    [EventHandler(2)]
    public async Task AddPositionAsync(AddStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position) is false)
        {
            var command = new AddPositionCommand(new(staffEvent.Staff.Position));
            await _eventBus.PublishAsync(command);
            staffEvent.Staff.PositionId = command.Result;
        }
    }

    [EventHandler(3)]
    public async Task AddStaffAsync(AddStaffDomainEvent staffEvent)
    {
        //var staffDto = staffEvent.Staff;
        //var staff = await _staffRepository.FindAsync(s => s.JobNumber == staffDto.JobNumber);
        //if (staff is not null)
        //    throw new UserFriendlyException($"Staff with jobNumber number {staffDto.JobNumber} already exists");

        //staff = new Staff(staffDto.UserId, staffDto.,staffDto.JobNumber, staffDto.User.Name, staffDto.PositionId, staffDto.StaffType, staffDto.Enabled);
        //staff.AddDepartmentStaff(staffDto.DepartmentId);
        //staff.AddTeamStaff(staffDto.Teams);
        //await _staffRepository.AddAsync(staff);
    }
}
