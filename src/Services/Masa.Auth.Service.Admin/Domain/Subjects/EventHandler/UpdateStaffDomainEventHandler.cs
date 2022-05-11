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

    [EventHandler(1)]
    public async Task UpdateUserAsync(UpdateStaffDomainEvent staffEvent)
    {
        var command = new UpdateUserCommand(staffEvent.Staff.User);
        await _eventBus.PublishAsync(command);
    }

    [EventHandler(2)]
    public async Task UpsertPositionAsync(UpdateStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position.Name) is false)
        {
            var command = new UpsertPositionCommand(staffEvent.Staff.Position);
            await _eventBus.PublishAsync(command);
        }
    }

    [EventHandler(3)]
    public async Task UpdateStaffAsync(UpdateStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var staff = await _staffRepository.FindAsync(s => s.Id == staffDto.Id);
        if (staff is null)
            throw new UserFriendlyException("This staff data does not exist");

        staff.Update(staffDto.User.Name, staffDto.Position.Id, staffDto.StaffType, staffDto.Enabled);
        staff.AddDepartmentStaff(staffDto.DepartmentId);
        staff.AddTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);
    }
}
