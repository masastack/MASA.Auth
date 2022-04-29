// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamRole : AuditEntity<Guid, Guid>, ISoftDelete
{
    public Guid TeamId { get; private set; }

    public Team Team { get; private set; } = null!;

    public bool IsDeleted { get; private set; }

    public Guid RoleId { get; private set; }

    public Role Role { get; private set; } = null!;

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamRole(Guid roleId, TeamMemberTypes teamMemberType)
    {
        RoleId = roleId;
        TeamMemberType = teamMemberType;
    }
}

