// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientPaginationListQuery(int Page, int PageSize, string SearchKey) : Query<PaginationDto<ClientDto>>
{
    public override PaginationDto<ClientDto> Result { get; set; } = new();
}
