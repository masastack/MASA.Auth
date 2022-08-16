// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserDto
{
    public Guid Id { get; set; }

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string? IdCard { get; set; }

    public string Account { get; set; }

    public string? CompanyName { get; set; }

    public bool Enabled { get; set; }

    public string? PhoneNumber { get; set; }

    public string? Email { get; set; }

    public AddressValueDto Address { get; set; }

    public DateTime CreationTime { get; set; }

    public GenderTypes Gender { get; set; }

    public string? Landline { get; set; }

    public UserDto()
    {
        DisplayName = "";
        Avatar = "";
        Account = "";
        Address = new();
    }

    public UserDto(Guid id, string? name, string displayName, string avatar, string? idCard, string account, string? companyName, bool enabled, string? phoneNumber, string? email, AddressValueDto address, DateTime creationTime, GenderTypes gender, string? landline)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        CompanyName = companyName;
        Enabled = enabled;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        CreationTime = creationTime;
        Gender = gender;
        Landline = landline;
    }
}

