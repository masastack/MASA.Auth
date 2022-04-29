// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class CopyDepartmentDto : UpsertDepartmentDto
{
    public List<StaffDto> Staffs { get; set; } = new();

    public override List<Guid> StaffIds => Staffs.Select(s => s.Id).ToList();

    public bool MigrateStaff { get; set; }

    public Guid SourceId { get; set; }
}

