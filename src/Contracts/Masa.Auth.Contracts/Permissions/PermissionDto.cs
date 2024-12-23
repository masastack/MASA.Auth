// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class PermissionDto
{
    public Guid Id { get; set; }

    public PermissionTypes Type { get; set; }

    public string Name { get; set; } = "";
}
