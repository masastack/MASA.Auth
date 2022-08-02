// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class AddStaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly IUserRepository _userRepository;
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly IEventBus _eventBus;

    public AddStaffDomainEventHandler(
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
        var user = await _userRepository.FindAsync(u => u.PhoneNumber == staffDto.PhoneNumber);
        if (user is null)
        {
            var command = new AddUserCommand(new AddUserDto(staffDto.Name, staffDto.DisplayName, staffDto.Avatar, staffDto.IdCard, staffDto.CompanyName, staffDto.Enabled, staffDto.PhoneNumber, staffDto.Email, staffDto.Address, staffDto.Department, staffDto.Position ?? "", staffDto.Account, staffDto.Password, staffDto.Gender, new(), new()));
            await _eventBus.PublishAsync(command);
            staffEvent.Staff.UserId = command.NewUser.Id;
        }
        else
        {
            staffEvent.Staff.UserId = user.Id;
        }
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
        Expression<Func<Staff, bool>> condition = staff => staff.Account == staffDto.Account || staff.JobNumber == staffDto.JobNumber;
        if (!string.IsNullOrEmpty(staffDto.PhoneNumber))
            condition = condition.Or(staff => staff.PhoneNumber == staffDto.PhoneNumber);
        if (!string.IsNullOrEmpty(staffDto.Email))
            condition = condition.Or(staff => staff.Email == staffDto.Email);
        if (!string.IsNullOrEmpty(staffDto.IdCard))
            condition = condition.Or(staff => staff.IdCard == staffDto.IdCard);

        var staff = await _staffRepository.FindAsync(condition);
        if (staff is not null)
        {
            if (string.IsNullOrEmpty(staffDto.PhoneNumber) is false && staffDto.PhoneNumber == staff.PhoneNumber)
                throw new UserFriendlyException($"Staff with phone number {staffDto.PhoneNumber} already exists");
            if (string.IsNullOrEmpty(staffDto.Account) is false && staffDto.Account == staff.Account)
                throw new UserFriendlyException($"Staff with account {staffDto.Account} already exists");
            if (string.IsNullOrEmpty(staffDto.Email) is false && staffDto.Email == staff.Email)
                throw new UserFriendlyException($"Staff with email {staffDto.Email} already exists");
            if (string.IsNullOrEmpty(staffDto.IdCard) is false && staffDto.IdCard == staff.IdCard)
                throw new UserFriendlyException($"Staff with email {staffDto.IdCard} already exists");
            if (string.IsNullOrEmpty(staffDto.JobNumber) is false && staffDto.JobNumber == staff.JobNumber)
                throw new UserFriendlyException($"Staff with jobNumber number {staffDto.JobNumber} already exists");
        }

        staff = new Staff(
            staffDto.UserId, staffDto.Name, staffDto.DisplayName, staffDto.Avatar,
            staffDto.IdCard, staffDto.Account, staffDto.Password, staffDto.CompanyName,
            staffDto.Gender, staffDto.PhoneNumber, staffDto.Email, staffDto.Address,
            staffDto.JobNumber, staffDto.PositionId, staffDto.StaffType, staffDto.Enabled);
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
}
