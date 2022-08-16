// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UpsertStaffDto : AddStaffDto
{
    public UpsertStaffDto()
    {

    }

    public UpsertStaffDto(string jobNumber, StaffTypes staffType, bool enabled, Guid departmentId, Guid positionId, string position, List<Guid> teams, Guid userId, string name, string displayName, string avatar, string idCard, string companyName, string phoneNumber, string landline, string email, AddressValueDto address, string department, string account, string password, GenderTypes gender) : base(jobNumber, staffType, enabled, departmentId, positionId, position, teams, userId, name, displayName, avatar, idCard, companyName, phoneNumber, landline, email, address, password, gender)
    {
    }
}
