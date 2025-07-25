﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddUserDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string? DisplayName { get; set; }

    public string? Avatar { get; set; }

    public string? IdCard { get; set; }

    public string? CompanyName { get; set; }

    public bool Enabled { get; set; } = true;

    public string? PhoneNumber { get; set; }

    public string? Landline { get; set; }

    public string? Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string? Department { get; set; }

    public string? Position { get; set; }

    public string? Account { get; set; }

    public string? Password { get; set; }

    public GenderTypes Gender { get; set; }

    public List<Guid> Roles { get; set; }

    public List<SubjectPermissionRelationDto> Permissions { get; set; }

    public PasswordTypes PasswordType { get; set; }

    /// <summary>
    /// Client ID, used to record which client the user registered from
    /// </summary>
    public string? ClientId { get; set; }

    public AddUserDto()
    {
        Address = new();
        Gender = GenderTypes.Male;
        Roles = new();
        Permissions = new();
        PasswordType = PasswordTypes.MD5;
    }

    public AddUserDto(Guid id,string? name, string? displayName, string? avatar, string? idCard, string? companyName, bool enabled, string? phoneNumber, string? landline, string? email, AddressValueDto address, string? department, string? position, string? account, string? password, GenderTypes gender, List<Guid>? roles, List<SubjectPermissionRelationDto>? permissions, PasswordTypes passwordType, string? clientId = null)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Landline = landline;
        Email = email;
        Address = address;
        Department = department;
        Position = position;
        Account = account;
        Password = password;
        Gender = gender;
        Roles = roles ?? new();
        Permissions = permissions ?? new();
        PasswordType = passwordType;
        ClientId = clientId;
    }
}
