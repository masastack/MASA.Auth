// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class RemoveRoleDto
{
    public Guid Id { get; set; }

    public RemoveRoleDto(Guid id)
    {
        Id = id;
    }
}

