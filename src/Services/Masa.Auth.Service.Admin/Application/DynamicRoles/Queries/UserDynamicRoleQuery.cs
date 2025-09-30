// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles.Queries;

public record UserDynamicRoleQuery(Guid UserId, List<Guid> RoleIds) : Query<List<DynamicRoleDto>>
{
    public override List<DynamicRoleDto> Result { get; set; } = new();
}
