// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class DepartmentDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<StaffDto> StaffList { get; set; } = new();
}

