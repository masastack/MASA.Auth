// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleDetailExternalQuery(Guid RoleId) : Query<RoleSimpleDetailDto>
{
    public override RoleSimpleDetailDto Result { get; set; } = null!;
}
