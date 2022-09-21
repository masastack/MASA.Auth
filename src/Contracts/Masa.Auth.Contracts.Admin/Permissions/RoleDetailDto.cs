// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleDetailDto : RoleDto
{
    public List<SubjectPermissionRelationDto> Permissions { get; set; }

    public List<Guid> ParentRoles { get; set; }

    public List<Guid> ChildrenRoles { get; set; }

    public List<UserSelectDto> Users { get; set; }

    public List<Guid> Teams { get; set; }

    public int AvailableQuantity { get; set; }

    public RoleDetailDto() : base()
    {
        Permissions = new();
        ParentRoles = new();
        ChildrenRoles = new();
        Users = new();
        Teams = new();
    }

    public RoleDetailDto(Guid id, string name, string code, int limit, string? description, bool enabled, DateTime creationTime, DateTime? modificationTime, string creator, string modifier, List<SubjectPermissionRelationDto> permissions, List<Guid> parentRoles, List<Guid> childrenRoles, List<UserSelectDto> users, List<Guid> teams, int availableQuantity) : base(id, name, code, limit, description, enabled, creationTime, modificationTime, creator, modifier)
    {
        Permissions = permissions;
        ParentRoles = parentRoles;
        ChildrenRoles = childrenRoles;
        Users = users;
        Teams = teams;
        AvailableQuantity = availableQuantity;
    }
}


