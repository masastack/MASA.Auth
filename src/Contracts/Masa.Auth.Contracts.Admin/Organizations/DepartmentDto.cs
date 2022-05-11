// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class DepartmentDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public List<DepartmentDto> Children { get; set; } = new();

    public bool IsRoot { get; set; }
}

