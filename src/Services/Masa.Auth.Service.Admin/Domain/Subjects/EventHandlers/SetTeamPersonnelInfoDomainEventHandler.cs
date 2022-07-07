// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandlers;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly ITeamRepository _teamRepository;

    public SetTeamPersonnelInfoDomainEventHandler(ITeamRepository teamRepository)
    {
        _teamRepository = teamRepository;
    }

    [EventHandler(2)]
    public void SetUser(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        setTeamPersonnelInfoDomainEvent.Team.SetStaff(setTeamPersonnelInfoDomainEvent.Type, setTeamPersonnelInfoDomainEvent.StaffIds);
    }

    [EventHandler(4)]
    public async Task SaveTeamAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        await _teamRepository.UpdateAsync(setTeamPersonnelInfoDomainEvent.Team);
    }
}
