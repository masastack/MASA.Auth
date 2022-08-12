// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Permissions;

public class GetPermissionsByTeamWithUserDto
{
    public Guid User { get; set; }

    public List<Guid> Teams { get; set; } = new ();

    public GetPermissionsByTeamWithUserDto(Guid user, List<Guid> teams)
    {
        User = user;
        Teams = teams;
    }
}
