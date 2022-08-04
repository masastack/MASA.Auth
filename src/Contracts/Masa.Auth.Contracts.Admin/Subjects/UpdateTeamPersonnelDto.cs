// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateTeamPersonnelDto
{
    public Guid Id { get; set; }

    public List<Guid> Staffs { get; set; } = new();

    public List<Guid> Roles { get; set; } = new();

    public List<SubjectPermissionRelationDto> Permissions { get; set; } = new();
}
