// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Webhooks;

public class QueryHandler
{
    readonly IWebhookRepository _webhookRepository;

    public QueryHandler(IWebhookRepository webhookRepository)
    {
        _webhookRepository = webhookRepository;
    }

    [EventHandler]
    public async Task GetWebhookDetailAsync(WebhookDetailQuery query)
    {
        var item = await _webhookRepository.FindAsync(query.Id);
        if (item is null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.WEBHOOK_NOT_EXIST);
        }
        query.Result = item.Adapt<WebhookDetailDto>();
    }

    [EventHandler]
    public async Task GetWebhookListAsync(WebhookListQuery query)
    {
        Expression<Func<Webhook, bool>> condition = webhook => true;
        if (!string.IsNullOrWhiteSpace(query.Name))
            condition = condition.And(w => w.Name.Contains(query.Name));

        var result = await _webhookRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Webhook.ModificationTime)] = true,
            }
        });
        query.Result = new PaginationDto<WebhookItemDto>(result.Total, result.Result.Adapt<List<WebhookItemDto>>());
    }
}
