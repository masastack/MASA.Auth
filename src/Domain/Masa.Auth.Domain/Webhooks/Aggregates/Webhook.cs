// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Domain.Webhooks.Aggregates;

public class Webhook : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; set; } = "";

    public string Url { get; private set; } = "";

    public string HttpMethod { get; private set; } = "POST";

    public string? Secret { get; private set; }

    public string? Description { get; private set; }

    public bool IsActive { get; private set; } = true;

    public WebhookEvent Event { get; private set; }

    private List<WebhookLog> _webhookLogs = new();

    public IReadOnlyCollection<WebhookLog> WebhookLogs => _webhookLogs;

    public Webhook(string name, string url, string httpMethod, string secret, string description, bool isActive, WebhookEvent @event)
    {
        Name = name;
        Url = url;
        HttpMethod = httpMethod;
        Secret = secret;
        Description = description;
        IsActive = isActive;
        Event = @event;
    }

    public void Update(string name, string url, string? secret, string? description, bool isActive, WebhookEvent @event)
    {
        Name = name;
        Url = url;
        Secret = secret ?? Secret;
        Description = description ?? Description;
        IsActive = isActive;
        Event = @event;
    }

    public void AddLog(WebhookLog webhookLog)
    {
        _webhookLogs.Add(webhookLog);
    }
}
