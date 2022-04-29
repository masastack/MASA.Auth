﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddUserDto
{
    public string Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string IdCard { get; set; }

    public string CompanyName { get; set; }

    public bool Enabled { get; set; } = true;

    public string PhoneNumber { get; set; }

    public string Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string Department { get; set; }

    public string Position { get; set; }

    public string Account { get; set; }

    public string Password { get; set; }

    public GenderTypes Gender { get; set; }

    public List<Guid> Roles { get; set; }

    public List<UserPermissionDto> Permissions { get; set; }

    public AddUserDto()
    {
        Name = "";
        DisplayName = "";
        Avatar = "";
        IdCard = "";
        CompanyName = "";
        PhoneNumber = "";
        Email = "";
        Address = new();
        Department = "";
        Position = "";
        Account = "";
        Password = "";
        Gender = GenderTypes.Male;
        Roles = new List<Guid>();
        Permissions = new List<UserPermissionDto>();
    }

    public AddUserDto(string name, string displayName, string avatar, string idCard, string companyName, bool enabled, string phoneNumber, string email, AddressValueDto address, string department, string position, string account, string password, GenderTypes genderType, List<Guid> roleIds, List<UserPermissionDto> permissions)
    {
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Account = account;
        Password = password;
        Gender = genderType;
        Roles = roleIds;
        Permissions = permissions;
    }
}
