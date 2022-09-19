// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Auth.Contracts.Model;

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto : StaffDto
{
    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public List<Guid> TeamIds { get; set; } = new();

    public List<string> ThirdPartyIdpAvatars { get; set; } = new();

    public string Creator { get; set; } = "";

    public string Modifier { get; set; } = "";

    public DateTime? ModificationTime { get; set; }

    public List<RoleModel> Roles { get; set; } = new();

    public List<SubjectPermissionRelationDto> Permissions { get; set; } = new();

    public StaffDetailDto()
    {
    }

    [JsonConstructor]
    public StaffDetailDto(Guid departmentId, Guid positionId, List<Guid> teamIds, List<string> thirdPartyIdpAvatars, string creator, string modifier, DateTime? modificationTime, List<RoleModel> roles, List<SubjectPermissionRelationDto> permissions, Guid id, Guid userId, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, string name, string displayName, string avatar, string idCard, string companyName, string phoneNumber, string email, AddressValueDto address, DateTime creationTime, GenderTypes gender) : base(id, userId, department, position, jobNumber, enabled, staffType, name, displayName, avatar, idCard, companyName, phoneNumber, email, address, creationTime, gender)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
        TeamIds = teamIds;
        ThirdPartyIdpAvatars = thirdPartyIdpAvatars;
        Creator = creator;
        Modifier = modifier;
        ModificationTime = modificationTime;
        Roles = roles;
        Permissions = permissions;
    }
}


