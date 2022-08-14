// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpdateStaffDto : AddStaffDto
{
    public Guid Id { get; set; }

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
        return new UpdateStaffDto(staff.Id, staff.JobNumber, staff.StaffType, staff.Enabled, staff.DepartmentId, staff.PositionId, staff.Position, new(staff.TeamIds), staff.Name, staff.DisplayName, staff.Avatar, staff.IdCard, staff.CompanyName, staff.PhoneNumber, staff.Email, staff.Address, staff.Gender, new(staff.UserId, staff.RoleIds, staff.Permissions));
    }
}
