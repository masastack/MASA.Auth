// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Message;

public class ClientMessageTemplateProvider : IClientMessageTemplateProvider, IScopedDependency
{
    private readonly IClientConfigRepository _clientConfigRepository;

    public ClientMessageTemplateProvider(IClientConfigRepository clientConfigRepository)
    {
        _clientConfigRepository = clientConfigRepository;
    }

    public async Task<(string channelCode, string templateCode)?> ResolveAsync(string? clientId, ChannelTypes channelType, int scene)
    {
        if (string.IsNullOrEmpty(clientId))
        {
            return null;
        }

        var clientConfig = await _clientConfigRepository.FindByClientIdAsync(clientId);
        var template = clientConfig?.MessageTemplates
            .FirstOrDefault(t => t.ChannelType == channelType && t.Scene == scene);

        if (template is null || string.IsNullOrEmpty(template.ChannelCode) || string.IsNullOrEmpty(template.TemplateCode))
        {
            return null;
        }

        return (template.ChannelCode, template.TemplateCode);
    }
}
