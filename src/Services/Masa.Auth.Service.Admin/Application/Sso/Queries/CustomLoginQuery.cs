// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record CustomLoginQuery(int Page, int PageSize, string Search) : Query<PaginationDto<CustomLoginDto>>
{
    public override PaginationDto<CustomLoginDto> Result { get; set; } = new();
}
