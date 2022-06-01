// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class RoleRelation : FullEntity<Guid, Guid>
{
    public Guid ParentId { get; private set; }

    public Role ParentRole { get; set; } = null!;

    public Guid RoleId { get; private set; }

    public Role Role { get; set; } = null!;

    public RoleRelation(Guid parentId)
    {
        ParentId = parentId;
    }

    public RoleRelation(Guid roleId, Guid parentId)
    {
        RoleId = roleId;
    }
}

