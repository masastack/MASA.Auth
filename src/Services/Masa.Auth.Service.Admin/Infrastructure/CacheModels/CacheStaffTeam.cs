// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.CacheModels;

public class CacheStaffTeam
{
    public CacheStaffTeam(Guid id, TeamMemberTypes teamMemberType)
    {
        Id = id;
        TeamMemberType = teamMemberType;
    }

    public Guid Id { get; set; }
    public TeamMemberTypes TeamMemberType { get; set; }
}
