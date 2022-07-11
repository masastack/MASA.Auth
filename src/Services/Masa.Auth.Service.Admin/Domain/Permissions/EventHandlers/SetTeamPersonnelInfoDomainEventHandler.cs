// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.EventHandlers;

public class SetTeamPersonnelInfoDomainEventHandler
{
    readonly IRoleRepository _roleRepository;

    public SetTeamPersonnelInfoDomainEventHandler(IRoleRepository roleRepository)
    {
        _roleRepository = roleRepository;
    }

    [EventHandler(1)]
    public void SetRole(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        setTeamPersonnelInfoDomainEvent.Team.SetRole(setTeamPersonnelInfoDomainEvent.Type, setTeamPersonnelInfoDomainEvent.RoleIds.ToArray());
    }

    [EventHandler(3)]
    public void SetPermission(SetTeamPersonnelInfoDomainEvent setTeamPersonnelInfoDomainEvent)
    {
        setTeamPersonnelInfoDomainEvent.Team.SetPermission(setTeamPersonnelInfoDomainEvent.Type
                , setTeamPersonnelInfoDomainEvent.PermissionsIds);
    }
}
