// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Permissions.Aggregates;

public class RolePermission : SubjectPermissionRelation
{
    public Guid RoleId { get; private set; }

    public Role Role { get; private set; } = default!;

    public RolePermission(Guid permissionId, bool effect) : base(permissionId, effect)
    {
    }
}

