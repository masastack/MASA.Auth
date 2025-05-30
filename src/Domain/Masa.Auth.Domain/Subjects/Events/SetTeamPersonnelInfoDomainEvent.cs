﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Events;

public record SetTeamPersonnelInfoDomainEvent(Team Team, TeamMemberTypes Type, List<Guid> StaffIds,
    List<Guid> RoleIds, List<SubjectPermissionRelationDto> Permissionss)
        : Event;
