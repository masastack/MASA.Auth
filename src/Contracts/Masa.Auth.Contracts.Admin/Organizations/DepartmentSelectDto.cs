// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Organizations;

public class DepartmentSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public DepartmentSelectDto()
    {

    }

    public DepartmentSelectDto(Guid id, string name)
    {
        Id = id;
        Name = name;
    }
}

