// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly ITeamRepository _teamRepository;
    readonly IStaffRepository _staffRepository;

    public SetTeamPersonnelInfoDomainEventHandler(ITeamRepository teamRepository, IStaffRepository staffRepository)
    {
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
    }

    [EventHandler(2)]
    public async Task SetUser(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        var staffs = await _staffRepository.GetListAsync(staff => setTeamPersonnelInfoDomainEvent.StaffIds.Contains(staff.Id));
        setTeamPersonnelInfoDomainEvent.Team.SetStaff(setTeamPersonnelInfoDomainEvent.Type, staffs);
    }

    [EventHandler(4)]
    public async Task SaveTeamAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        await _teamRepository.UpdateAsync(setTeamPersonnelInfoDomainEvent.Team);
    }
}
