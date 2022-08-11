// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamSampleDto
{
    public Guid Id { get; set; }

    public TeamMemberTypes TeamMemberType { get; set; }

    public TeamSampleDto(Guid id, TeamMemberTypes teamMemberType)
    {
        Id = id;
        TeamMemberType = teamMemberType;
    }
}
