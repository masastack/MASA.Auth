// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Webhooks;

public class CommandHandler
{
    readonly IWebhookRepository _webhookRepository;

    public CommandHandler(IWebhookRepository webhookRepository)
    {
        _webhookRepository = webhookRepository;
    }

    [EventHandler]
    public async Task SaveWebhookAsync(SaveWebhookCommand command)
    {
        var dto = command.WebhookDto;
        if (dto.Id == Guid.Empty)
        {
            await _webhookRepository.AddAsync(new Webhook(dto.Name, dto.Url, dto.HttpMethod, dto.Secret ?? "", dto.Description ?? "", dto.IsActive, dto.Event));
        }
        else
        {
            var item = await _webhookRepository.FindAsync(dto.Id);
            if (item == null)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.WEBHOOK_NOT_EXIST);
            }
            item.Update(dto.Name, dto.Url, dto.Secret, dto.Description, dto.IsActive, dto.Event);
            await _webhookRepository.UpdateAsync(item);
        }
    }

    [EventHandler]
    public async Task RemoveWebhookAsync(RemoveWebhookCommand command)
    {
        var item = await _webhookRepository.FindAsync(command.Id);
        if (item == null)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.WEBHOOK_NOT_EXIST);
        }
        await _webhookRepository.RemoveAsync(item);
    }
}
