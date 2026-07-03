// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Message;

/// <summary>
/// Resolves the (channel code, template code) a client has configured for a given
/// (channel type, scene). Returns null when the client has no matching configuration,
/// signalling the caller to fall back to the global DCC SmsOptions/EmailOptions.
/// </summary>
public interface IClientMessageTemplateProvider
{
    Task<(string channelCode, string templateCode)?> ResolveAsync(string? clientId, ChannelTypes channelType, int scene);
}
