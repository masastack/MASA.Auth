// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class StaffDomainEventHandler
{
    readonly IStaffRepository _staffRepository;
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly IEventBus _eventBus;

    public StaffDomainEventHandler(
        IStaffRepository staffRepository,
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService,
        IEventBus eventBus)
    {
        _staffRepository = staffRepository;
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
        _eventBus = eventBus;
    }

    [EventHandler(1)]
    public async Task AddUserAsync(AddStaffBeforeDomainEvent staffEvent)
    {
        var command = new AddUserCommand(staffEvent.User, true);
        await _eventBus.PublishAsync(command);
        staffEvent.UserId = command.NewUser.Id;
    }

    [EventHandler(2)]
    public async Task UpsertPositionAsync(AddStaffBeforeDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Position) is false)
        {
            var command = new UpsertPositionCommand(new(staffEvent.Position));
            await _eventBus.PublishAsync(command);
            staffEvent.PositionId = command.Result;
        }
    }

    [EventHandler(1)]
    public async Task UpdateRoleLimitAsync(AddStaffAfterDomainEvent staffEvent)
    {
        var teams = staffEvent.Staff.TeamStaffs.Select(team => team.TeamId).ToList();
        if (teams.Count > 0)
        {
            await UpdateRoleLimitAsync(teams);
        }
    }
   
    [EventHandler(1)]
    public async Task UpsertPositionAsync(UpdateStaffBeforeDomainEvent staffEvent)
    {
        if (string.IsNullOrEmpty(staffEvent.Position) is false)
        {
            var command = new UpsertPositionCommand(new(staffEvent.Position));
            await _eventBus.PublishAsync(command);
            staffEvent.PositionId = command.Result;
        }
    }

    [EventHandler(1)]
    public async Task UpdateRoleLimitAsync(UpdateStaffAfterDomainEvent staffEvent)
    {
        if (staffEvent.Teams?.Count > 0)
        {
            await UpdateRoleLimitAsync(staffEvent.Teams);
        }
    }

    [EventHandler(1)]
    public async Task UpdateRoleLimitAsync(RemoveStaffDomainEvent staffEvent)
    {
        var teams = staffEvent.Staff.TeamStaffs.Select(team => team.TeamId).ToList();
        await UpdateRoleLimitAsync(teams);
    }

    async Task UpdateRoleLimitAsync(List<Guid> teams)
    {
        if (teams.Count > 0)
        {
            var roles = await _authDbContext.Set<TeamRole>()
                                .Where(tr => teams.Contains(tr.TeamId))
                                .Select(tr => tr.RoleId)
                                .ToListAsync();
            await _roleDomainService.UpdateRoleLimitAsync(roles);
        }
    }
}
