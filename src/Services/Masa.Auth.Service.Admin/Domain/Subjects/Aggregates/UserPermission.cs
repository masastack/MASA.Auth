// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserPermission : SubjectPermissionRelation
{
    public Guid UserId { get; private set; }

    public User User { get; private set; } = default!;

    public UserPermission(Guid permissionId, bool effect) : base(permissionId, effect)
    {
    }
}

