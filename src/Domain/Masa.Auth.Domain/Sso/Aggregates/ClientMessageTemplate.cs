// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.BuildingBlocks.StackSdks.Mc.Enum;

namespace Masa.Auth.Domain.Sso.Aggregates;

/// <summary>
/// Client-level message template binding for a single (channel type, scene).
/// Scene stores the value of SendMsgCodeTypes (when ChannelType is Sms) or
/// SendEmailTypes (when ChannelType is Email); ChannelType disambiguates the two.
/// </summary>
public class ClientMessageTemplate : Entity<int>
{
    public int ClientConfigId { get; private set; }

    public ChannelTypes ChannelType { get; private set; }

    public int Scene { get; private set; }

    public string ChannelCode { get; private set; } = "";

    public string TemplateCode { get; private set; } = "";

    public ClientMessageTemplate(ChannelTypes channelType, int scene, string channelCode, string templateCode)
    {
        ChannelType = channelType;
        Scene = scene;
        ChannelCode = channelCode;
        TemplateCode = templateCode;
    }
}
