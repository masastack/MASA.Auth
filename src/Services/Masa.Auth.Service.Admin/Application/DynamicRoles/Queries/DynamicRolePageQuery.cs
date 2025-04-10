// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.DynamicRoles.Queries;

public record DynamicRolePageQuery(GetDynamicRoleInput Options) : Query<PaginationDto<DynamicRoleDto>>
{
    public override PaginationDto<DynamicRoleDto> Result { get; set; } = new();
}
