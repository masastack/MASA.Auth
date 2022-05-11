// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class PermissionRelation : AuditEntity<Guid, Guid>, ISoftDelete
{
    public Guid ChildPermissionId { get; private set; }

    public Guid ParentPermissionId { get; private set; }

    public bool IsDeleted { get; private set; }

    public Permission ChildPermission { get; private set; } = null!;

    public Permission ParentPermission { get; private set; } = null!;

    public PermissionRelation(Guid parentPermissionId, Guid childPermissionId)
    {
        ParentPermissionId = parentPermissionId;
        ChildPermissionId = childPermissionId;
    }
}

