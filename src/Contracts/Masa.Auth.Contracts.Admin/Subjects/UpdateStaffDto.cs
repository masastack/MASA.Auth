// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateStaffDto
{
    public Guid Id { get; set; }

    public string JobNumber { get; set; } = "";

    public StaffTypes StaffType { get; set; }

    public bool Enabled { get; set; }

    public Guid DepartmentId { get; set; }

    public Guid PositionId { get; set; }

    public string Position { get; set; } = "";

    public List<Guid> Teams { get; set; } = new();

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Avatar { get; set; } = "";

    public string IdCard { get; set; } = "";

    public string CompanyName { get; set; } = "";

    public string PhoneNumber { get; set; } = "";

    public string Landline { get; set; } = "";

    public string Email { get; set; } = "";

    public AddressValueDto Address { get; set; } = new();

    public GenderTypes Gender { get; set; }

    public UpdateUserAuthorizationDto User { get; set; } = new();

    public UpdateStaffDto()
    {
    }

    public UpdateStaffDto(Guid id, string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teams, string name, string displayName, string avatar, string idCard, string companyName, string phoneNumber, string email, AddressValueDto address, GenderTypes gender, UpdateUserAuthorizationDto user)
    {
        Id = id;
        JobNumber = jobNumber;
        StaffType = staffType;
        Enabled = enabled;
        DepartmentId = departmentId;
        PositionId = positionId;
        Position = position;
        Teams = teams;
        Name = name;
        DisplayName = displayName;
        Avatar = avatar;
        IdCard = idCard;
        CompanyName = companyName;
        PhoneNumber = phoneNumber;
        Email = email;
        Address = address;
        Gender = gender;
        User = user;
    }

    public static implicit operator UpdateStaffDto(StaffDetailDto staff)
    {
        return new UpdateStaffDto(staff.Id, staff.JobNumber, staff.StaffType, staff.Enabled, staff.DepartmentId, staff.PositionId, staff.Position, staff.TeamIds, staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.Gender, new(staff.UserId, staff.RoleIds, staff.Permissions));
    }
}
