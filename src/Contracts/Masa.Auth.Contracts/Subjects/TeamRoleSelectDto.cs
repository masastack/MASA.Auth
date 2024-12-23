// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Subjects;

public class TeamRoleSelectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public List<RoleSelectDto> Roles { get; set; }

    public TeamRoleSelectDto(Guid id, string name, string avatar, List<RoleSelectDto> roles)
    {
        Id = id;
        Name = name;
        Avatar = avatar;
        Roles = roles;
    }
}
