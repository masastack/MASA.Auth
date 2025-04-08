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

    public RoleTypes Type { get; set; }

    public List<string> Clients { get; set; }

    public AddRoleDto()
    {
        Name = "";
        Code = "";
        Enabled = true;
        Permissions = new();
        ChildrenRoles = new();
        Clients = new();
        Limit = 0;
        Type = RoleTypes.Domain;
    }


    public AddRoleDto(string name, string code, string? description, bool enabled, int limit, RoleTypes type, List<SubjectPermissionRelationDto> permissions, List<Guid> childrenRoles, List<string> clients)
    {
        Name = name;
        Code = code;
        Description = description;
        Enabled = enabled;
        Limit = limit;
        Type = type;
        Permissions = permissions;
        ChildrenRoles = childrenRoles;
        Clients = clients;
    }
}

