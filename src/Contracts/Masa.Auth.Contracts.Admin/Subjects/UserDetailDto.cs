// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDetailDto : UserDto
{
    public AddressValueDto Address { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string Password { get; set; }

    public List<string> ThirdPartyIdpAvatars { get; set; }

    public string Creator { get; set; }

    public string Modifier { get; set; }

    public DateTime? ModificationTime { get; set; }

    public List<Guid> RoleIds { get; set; }

    public List<UserPermissionDto> Permissions { get; set; }

    public UserDetailDto() : base()
    {
        Address = new();
        ThirdPartyIdpAvatars = new();
        Creator = "";
        Modifier = "";
        Department = "";
        Position = "";
        Password = "";
        RoleIds = new();
        Permissions = new();
    }

    public UserDetailDto(Guid id, string name, string displayName, string avatar, string idCard, string account, string companyName, bool enabled, string phoneNumber, string email, DateTime creationTime, AddressValueDto address, List<string> thirdPartyIdpAvatars, string creator, string modifier, DateTime? modificationTime, string department, string position, string password, GenderTypes genderType, List<Guid> roleIds, List<UserPermissionDto> permissions) : base(id, name, displayName, avatar, idCard, account, companyName, enabled, phoneNumber, email, creationTime, genderType)
    {
        Address = address;
        Department = department;
        Position = position;
        Password = password;
        ThirdPartyIdpAvatars = thirdPartyIdpAvatars;
        Creator = creator;
        Modifier = modifier;
        ModificationTime = modificationTime;
        RoleIds = roleIds;
        Permissions = permissions;
    }
}

