// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamStaff : FullEntity<Guid, Guid>
{
    public Guid TeamId { get; private set; }

    public Guid StaffId { get; private set; }

    public Staff Staff { get; private set; } = default!;

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamStaff(Guid staffId, TeamMemberTypes teamMemberType)
    {
        StaffId = staffId;
        TeamMemberType = teamMemberType;
    }

    public TeamStaff(Guid teamId, Guid staffId, TeamMemberTypes teamMemberType)
    {
        TeamId = teamId;
        TeamMemberType = teamMemberType;
    }
}


