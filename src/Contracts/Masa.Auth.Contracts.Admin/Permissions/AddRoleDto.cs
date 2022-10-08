// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class AddRoleDto
{
    public string Name { get; set; }

    public string Code { get; set; }

    public string? Description { get; set; }

    public bool Enabled { get; set; }

    public int Limit { get; set; }

    public List<SubjectPermissionRelationDto> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public AddRoleDto()
    {
        Name = "";
        Code = "";
        Enabled = true;
        Permissions = new();
        ChildrenRoles = new();
    }

    public AddRoleDto(string name, string code, string? description, bool enabled, int limit, List<SubjectPermissionRelationDto> permissions, List<Guid> childrenRoles)
    {
        Name = name;
        Code = code;
        Description = description;
        Enabled = enabled;
        Limit = limit;
        Permissions = permissions;
        ChildrenRoles = childrenRoles;
    }
}

