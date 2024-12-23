// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class MenuPermissionDetailDto : PermissionDetailDto
{
    public List<RoleSelectDto> Roles { get; set; } = new();

    public List<UserSelectDto> Users { get; set; } = new();

    public List<TeamSelectDto> Teams { get; set; } = new();
}