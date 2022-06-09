// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class AddRoleDto
{
    public string Name { get; set; }

    public string Description { get; set; }

    public bool Enabled { get; set; }

    public int Limit { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public AddRoleDto()
    {
        Name = "";
        Description = "";
        Enabled = true;
        Permissions = new();
        ChildrenRoles = new();
    }

    public AddRoleDto(string name, string description, bool enabled, int limit, List<Guid> rolePermissions, List<Guid> childRoles, List<Guid> users)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        Permissions = rolePermissions;
        ChildrenRoles = childRoles;
        Limit = limit;
    }
}

