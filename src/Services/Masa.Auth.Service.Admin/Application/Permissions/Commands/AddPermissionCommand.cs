// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Commands;

public record AddPermissionCommand(PermissionDetailDto PermissionDetail) : Command
{
    public bool Enabled { get; set; } = true;

    public Guid ParentId { get; set; }

    public List<Guid> ApiPermissions { get; set; } = new();
}
