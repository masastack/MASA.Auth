// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Webhooks.Aggregates;

public class WebhookLog : Entity<Guid>
{
    public Guid WebhookId { get; private set; }

    public Webhook Webhook { get; set; } = default!;

    public string Data { get; init; } = "";

    public WebhookLog(string data)
    {
        Data = data;
    }
}
