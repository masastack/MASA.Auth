// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class AppPermissionDto
{
    public string AppId { get; set; } = string.Empty;

    public PermissionTypes Type { get; set; }

    public Guid PermissionId { get; set; }

    public string PermissionName { get; set; } = string.Empty;

    public List<AppPermissionDto> Children { get; set; } = new();
}
