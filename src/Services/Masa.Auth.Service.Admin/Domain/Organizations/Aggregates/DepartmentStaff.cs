// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class DepartmentStaff : AuditEntity<Guid, Guid>, ISoftDelete
{
    public Guid DepartmentId { get; private set; }

    public Guid StaffId { get; private set; }

    public Department Department { get; private set; } = null!;

    public Staff Staff { get; private set; } = null!;

    public DepartmentStaff(Guid staffId)
    {
        StaffId = staffId;
    }

    public DepartmentStaff(Guid departmentId, Guid staffId)
    {
        DepartmentId = departmentId;
    }
}
