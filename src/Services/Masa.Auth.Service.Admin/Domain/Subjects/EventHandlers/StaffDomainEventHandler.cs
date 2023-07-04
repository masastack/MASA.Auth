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
    public async Task UpdateRoleLimitAsync(AddStaffAfterDomainEvent staffEvent)
    {
        var teams = staffEvent.Staff.TeamStaffs.Select(team => team.TeamId).ToList();
        if (teams.Count > 0)
        {
            await UpdateRoleLimitAsync(teams);
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
