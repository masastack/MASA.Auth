// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Events;

public record SetTeamPersonnelInfoDomainEvent(Team Team, TeamMemberTypes Type, string RoleName,
        List<Guid> StaffIds, List<Guid> RoleIds, Dictionary<Guid, bool> PermissionsIds)
        : Event;
