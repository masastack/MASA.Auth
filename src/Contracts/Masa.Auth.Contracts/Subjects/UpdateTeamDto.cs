// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateTeamDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public AvatarValueDto Avatar { get; set; } = new();

    public string Description { get; set; } = string.Empty;

    public TeamTypes Type { get; set; } = TeamTypes.Ordinary;

    public List<Guid> AdminStaffs { get; set; } = new();

    public List<Guid> AdminRoles { get; set; } = new();

    public List<SubjectPermissionRelationDto> AdminPermissions { get; set; } = new();

    public List<Guid> MemberStaffs { get; set; } = new();

    public List<Guid> MemberRoles { get; set; } = new();

    public List<SubjectPermissionRelationDto> MemberPermissions { get; set; } = new();
}
