// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiResourcesQuery(int Page, int PageSize, string Search) : Query<PaginationDto<ApiResourceDto>>
{
    public override PaginationDto<ApiResourceDto> Result { get; set; } = new();
}
