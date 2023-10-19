// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Webhooks.Queries;

public record WebhookListQuery(int Page, int PageSize, string Name) : Query<PaginationDto<WebhookItemDto>>
{
    public override PaginationDto<WebhookItemDto> Result { get; set; } = new();
}
