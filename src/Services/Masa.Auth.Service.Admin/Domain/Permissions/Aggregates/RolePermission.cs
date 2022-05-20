// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class RolePermission : FullAuditEntity<Guid, Guid>
{
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public RolePermission(Guid permissionId)
    {
        PermissionId = permissionId;
    }
}

