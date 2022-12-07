// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class AddStaffDto
{
    public string JobNumber { get; set; }

    public StaffTypes StaffType { get; set; } = StaffTypes.Internal;

    public bool Enabled { get; set; } = true;

    public Guid DepartmentId { get; set; }

    public string? Position { get; set; }

    public List<Guid> Teams { get; set; } = new();

    public string? Name { get; set; }

    public string DisplayName { get; set; }

    public string Avatar { get; set; }

    public string? IdCard { get; set; }

    public string? CompanyName { get; set; }

    public string PhoneNumber { get; set; }

    public string? Landline { get; set; }

    public string? Email { get; set; }

    public AddressValueDto Address { get; set; }

    public string? Password { get; set; }

    public GenderTypes Gender { get; set; } = GenderTypes.Male;

    public AddStaffDto()
    {
        JobNumber = "";
        DisplayName = "";
        PhoneNumber = "";
        Address = new();
        Avatar = "";
    }

    public AddStaffDto(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, string? position, List<Guid> teams, string? name, string displayName, string avatar, string? idCard, string? companyName, string phoneNumber, string? landline, string? email, AddressValueDto address, string? password, GenderTypes gender)
    {
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        Position = position;
        Teams = teams;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Landline = landline;
        Email = email;
        Address = address;
        Password = password;
        Gender = gender;
    }
}
