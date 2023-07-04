// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class StaffDomainEventHandler
{
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;

    public StaffDomainEventHandler(
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService)
    {
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
    }
#warning role limit
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
