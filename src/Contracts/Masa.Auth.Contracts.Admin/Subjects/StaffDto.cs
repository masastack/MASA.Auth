// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class StaffDto
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public string Department { get; set; } = "";

    public string Position { get; set; } = "";

    public string JobNumber { get; set; } = "";

    public bool Enabled { get; set; }

    public StaffTypes StaffType { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string Account { get; set; } = "";

    public string CompanyName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Email { get; set; } = "";

    public AddressValueDto Address { get; set; } = new();

    public DateTime CreationTime { get; set; }

    public GenderTypes Gender { get; set; }

    public StaffDto()
    {
    }

    public StaffDto(Guid id, Guid userId, string department, string position, string jobNumber, bool enabled, StaffTypes staffType, string name, string displayName, string avatar, string idCard, string account, string companyName, string phoneNumber, string email, AddressValueDto address, DateTime creationTime, GenderTypes gender)
    {
        Id = id;
        UserId = userId;
        Department = department;
        Position = position;
        JobNumber = jobNumber;
        Enabled = enabled;
        StaffType = staffType;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        Account = account;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        CreationTime = creationTime;
        Gender = gender;
    }
}


