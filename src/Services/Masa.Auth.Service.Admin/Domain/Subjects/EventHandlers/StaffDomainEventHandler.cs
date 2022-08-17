// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class StaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IUserRepository _userRepository;
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly IEventBus _eventBus;

    public StaffDomainEventHandler(
        IStaffRepository staffRepository,
        IUserRepository userRepository,
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService,
        IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _userRepository = userRepository;
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var addUserDto = new AddUserDto(staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName, staffDto.Enabled, staffDto.PhoneNumber, default, staffDto.Email, staffDto.Address, default, staffDto.Position, default, staffDto.Password, staffDto.Gender, default, default);
        var command = new AddUserCommand(addUserDto, true);
        await _eventBus.PublishAsync(command);
        staffEvent.Staff.UserId = command.NewUser.Id;
    }

    [EventHandler(2)]
    public async Task UpsertPositionAsync(AddStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position) is false)
        {
            var command = new UpsertPositionCommand(new(staffEvent.Staff.Position));
            await _eventBus.PublishAsync(command);
            staffEvent.Staff.PositionId = command.Result;
        }
    }

    [EventHandler(3)]
    public async Task AddStaffAsync(AddStaffDomainEvent staffEvent)
    {
        var staffDto = staffEvent.Staff;
        var staff = new Staff(
                staffDto.UserId, 
                staffDto.Name, 
                staffDto.DisplayName, 
                staffDto.Avatar,
                staffDto.IdCard, 
                staffDto.CompanyName,
                staffDto.Gender, 
                staffDto.PhoneNumber, 
                staffDto.Email,
                staffDto.JobNumber, 
                staffDto.PositionId, 
                staffDto.StaffType, 
                staffDto.Enabled, 
                staffDto.Address
            );
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.AddAsync(staff);

        var teams = staff.TeamStaffs.Select(team => team.TeamId).ToList();
        var roleIds = await _authDbContext.Set<TeamRole>()
                                    .Where(team => teams.Contains(team.TeamId))
                                    .Select(team => team.RoleId)
                                    .ToListAsync();
        await _roleDomainService.UpdateRoleLimitAsync(roleIds);
    }

    [EventHandler(2)]
    public async Task UpsertPositionAsync(UpdateStaffDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Staff.Position) is false)
        {
            var command = new UpsertPositionCommand(new(staffEvent.Staff.Position));
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

        staff.Update(
            staffDto.PositionId, staffDto.StaffType, staffDto.Enabled, staffDto.Name,
            staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName,
            staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Gender);
        staff.SetDepartmentStaff(staffDto.DepartmentId);
        var teams = staff.TeamStaffs.Select(team => team.TeamId).Union(staffDto.Teams).Distinct().ToList();
        staff.SetTeamStaff(staffDto.Teams);
        await _staffRepository.UpdateAsync(staff);

        var roleIds = await _authDbContext.Set<TeamRole>()
                                    .Where(team => teams.Contains(team.TeamId))
                                    .Select(team => team.RoleId)
                                    .ToListAsync();
        await _roleDomainService.UpdateRoleLimitAsync(roleIds);
    }
}
