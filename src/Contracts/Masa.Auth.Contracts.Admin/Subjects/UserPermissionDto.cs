// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class UserPermissionDto
{
    public Guid PermissionId { get; set; }

    public bool Effect { get; set; }

    public UserPermissionDto(Guid permissionId, bool effect)
    {
        PermissionId = permissionId;
        Effect = effect;
    }
}

