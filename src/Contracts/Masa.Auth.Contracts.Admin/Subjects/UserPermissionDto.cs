// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class PermissionSubjectRelationDto
{
    public Guid PermissionId { get; set; }

    public bool Effect { get; set; }

    public PermissionSubjectRelationDto(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }

    public override bool Equals(object? obj)
    {
        return obj is PermissionSubjectRelationDto psr && psr.PermissionId == PermissionId;
    }

    public override int GetHashCode()
    {
        return 1;
    }
}

