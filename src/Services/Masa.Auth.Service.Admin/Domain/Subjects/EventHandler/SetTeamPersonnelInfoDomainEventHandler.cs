// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.EventHandler;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly ITeamRepository _teamRepository;
    readonly IUserRepository _userRepository;
    readonly IStaffRepository _staffRepository;

    public SetTeamPersonnelInfoDomainEventHandler(ITeamRepository teamRepository, IUserRepository userRepository, IStaffRepository staffRepository)
    {
        _teamRepository = teamRepository;
        _userRepository = userRepository;
        _staffRepository = staffRepository;
    }

    [EventHandler(2)]
    public async Task SetUserAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        setTeamPersonnelInfoDomainEvent.Team.SetStaff(setTeamPersonnelInfoDomainEvent.Type, setTeamPersonnelInfoDomainEvent.StaffIds);
    }

    [EventHandler(4)]
    public async Task SaveTeamAsync(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        await _teamRepository.UpdateAsync(setTeamPersonnelInfoDomainEvent.Team);
    }
}
