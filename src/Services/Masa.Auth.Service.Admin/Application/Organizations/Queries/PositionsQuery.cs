// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations.Queries;

public record PositionsQuery(int Page, int PageSize, string Search) : Query<PaginationDto<PositionDto>>
{
    public override PaginationDto<PositionDto> Result { get; set; } = new();
}
