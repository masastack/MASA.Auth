// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class UpdateRoleDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Code { get; set; }

    public string? Description { get; set; }

    public bool Enabled { get; set; }

    public int Limit { get; set; }

    public List<SubjectPermissionRelationDto> Permissions { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public RoleTypes Type { get; set; }

    public List<string> Clients { get; set; }

    public UpdateRoleDto()
    {
        Name = "";
        Code = "";
        Permissions = new();
        ChildrenRoles = new();
        Clients = new();
    }

    public UpdateRoleDto(Guid id, string name,string code, string? description, bool enabled, int limit, RoleTypes type, List<SubjectPermissionRelationDto> permissions, List<Guid> childRoles, List<string> clients)
    {
        Id = id;
        Name = name;
        Code = code;
        Description = description;
        Enabled = enabled;
        Type = type;
        Permissions = permissions;
        ChildrenRoles = childRoles;
        Limit = limit;
        Clients = clients;
    }

    public static implicit operator UpdateRoleDto(RoleDetailDto role)
    {
        return new UpdateRoleDto(role.Id, role.Name, role.Code, role.Description, role.Enabled, role.Limit, role.Type, role.Permissions, role.ChildrenRoles, role.Clients);
    }
}

