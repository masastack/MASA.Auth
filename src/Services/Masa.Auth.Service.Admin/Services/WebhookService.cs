// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class WebhookService : ServiceBase
{
    public WebhookService() : base("api/webhook")
    {
        MapGet(GetAsync);
        MapGet(ListAsync);
        MapDelete(RemoveAsync);
        MapPost(SaveAsync);
    }

    private async Task SaveAsync(IEventBus eventBus, [FromBody] WebhookDetailDto webhookDto)
    {
        await eventBus.PublishAsync(new SaveWebhookCommand(webhookDto));
    }

    private async Task<WebhookDetailDto> GetAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new WebhookDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PaginationDto<WebhookItemDto>> ListAsync(IEventBus eventBus, GetWebhookPaginationDto paginationDto)
    {
        var query = new WebhookListQuery(paginationDto.Page, paginationDto.PageSize, paginationDto.Name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveWebhookCommand(id));
    }
}
