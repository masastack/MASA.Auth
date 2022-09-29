// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddTeamDto
{
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

    public static implicit operator AddTeamDto(TeamDetailDto team)
    {
        return new AddTeamDto()
        {
            Name = team.TeamBasicInfo.Name,
            Description = team.TeamBasicInfo.Description,
            Type = (TeamTypes)team.TeamBasicInfo.Type,
            Avatar = team.TeamBasicInfo.Avatar,
            AdminStaffs = team.TeamAdmin.Staffs,
            AdminRoles = team.TeamAdmin.Roles,
            AdminPermissions = team.TeamAdmin.Permissions,
            MemberRoles = team.TeamMember.Roles,
            MemberStaffs = team.TeamMember.Staffs,
            MemberPermissions = team.TeamMember.Permissions
        };
    }
}
