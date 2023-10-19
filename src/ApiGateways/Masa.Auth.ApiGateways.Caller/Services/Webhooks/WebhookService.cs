// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Webhooks;

public class WebhookService : ServiceBase
{
    protected override string BaseUrl { get; set; } = "";

    public WebhookService(ICaller caller) : base(caller)
    {
        BaseUrl = "api/webhook";
    }

    public async Task<PaginationDto<WebhookItemDto>> GetListAsync(GetWebhookPaginationDto getWebhookPaginationDto)
    {
        return await GetAsync<GetWebhookPaginationDto, PaginationDto<WebhookItemDto>>("list", getWebhookPaginationDto);
    }

    public async Task<WebhookDetailDto> GetDetailAsync(Guid id)
    {
        return await GetAsync<WebhookDetailDto>($"get?id={id}");
    }

    public async Task SaveAsync(WebhookDetailDto webhookDto)
    {
        await PostAsync("save", webhookDto);
    }

    public async Task RemoveAsync(Guid id)
    {
        await DeleteAsync($"remove?id={id}");
    }
}
