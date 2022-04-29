// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record GetRolesQuery(int Page, int PageSize, string Search, bool? Enabled) : Query<PaginationDto<RoleDto>>
{
    public override PaginationDto<RoleDto> Result { get; set; } = new();
}
