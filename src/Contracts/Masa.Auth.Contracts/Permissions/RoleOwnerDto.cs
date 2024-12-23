// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class RoleOwnerDto
{
    public List<UserSelectDto> Users { get; set; }

    public List<TeamSelectDto> Teams { get; set; }

    public RoleOwnerDto()
    {
        Users = new();
        Teams = new();
    }

    public RoleOwnerDto(List<UserSelectDto> users, List<TeamSelectDto> teams)
    {
        Users = users;
        Teams = teams;
    }
}

