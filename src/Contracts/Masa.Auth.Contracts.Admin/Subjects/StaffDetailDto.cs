// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDetailDto : StaffDto
{
    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public List<Guid> TeamIds { get; set; } = new();

    public string Password { get; set; } = "";

    public List<string> ThirdPartyIdpAvatars { get; set; } = new();

    public string Creator { get; set; } = "";

    public string Modifier { get; set; } = "";

    public DateTime? ModificationTime { get; set; }

    public List<Guid> RoleIds { get; set; } = new();

    public List<UserPermissionDto> Permissions { get; set; } = new();

    public StaffDetailDto()
    {
    }

    [JsonConstructor]
    public StaffDetailDto(Guid departmentId, Guid positionId, List<Guid> teamIds, string password, List<string> thirdPartyIdpAvatars, string creator, string modifier, DateTime? modificationTime, List<Guid> roleIds, List<UserPermissionDto> permissions,Guid id, Guid userId, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, string name, string displayName, string avatar, string idCard, string account, string companyName, string phoneNumber, string email, AddressValueDto address, DateTime creationTime, GenderTypes gender) : base(id, userId, department, position, jobNumber, enabled, staffType, name, displayName, avatar, idCard, account, companyName, phoneNumber, email, address, creationTime, gender)
    {
        DepartmentId = departmentId;
        PositionId = positionId;
        TeamIds = teamIds;
        Password = password;
        ThirdPartyIdpAvatars = thirdPartyIdpAvatars;
        Creator = creator;
        Modifier = modifier;
        ModificationTime = modificationTime;
        RoleIds = roleIds;
        Permissions = permissions;
    }
}


