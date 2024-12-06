// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Subjects.Aggregates;

public class TeamRole : FullEntity<Guid, Guid>
{
    public Guid TeamId { get; private set; }

    public Team Team { get; private set; } = null!;

    public Guid RoleId { get; private set; }

    public Role Role { get; private set; } = null!;

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamRole(Guid roleId, TeamMemberTypes teamMemberType)
    {
        RoleId = roleId;
        TeamMemberType = teamMemberType;
    }
}

