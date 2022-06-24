// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddStaffDto
{
    public string JobNumber { get; set; } = "";

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; } = true;

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; } = "";

    public List<Guid> Teams { get; set; } = new();

    public Guid UserId { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string CompanyName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Landline { get; set; } = "";

    public string Email { get; set; } = "";

    public AddressValueDto Address { get; set; } = new();

    public string Department { get; set; } = "";

    public string Account { get; set; } = "";

    public string Password { get; set; } = "";

    public GenderTypes Gender { get; set; } = GenderTypes.Male;

    public AddStaffDto()
    {
    }

    public AddStaffDto(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teams, Guid userId, string name, string displayName, string avatar, string idCard, string companyName, string phoneNumber, string landline, string email, AddressValueDto address, string department, string account, string password, GenderTypes gender)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        Teams = teams;
        UserId = userId;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Landline = landline;
        Email = email;
        Address = address;
        Department = department;
        Account = account;
        Password = password;
        Gender = gender;
    }
}
