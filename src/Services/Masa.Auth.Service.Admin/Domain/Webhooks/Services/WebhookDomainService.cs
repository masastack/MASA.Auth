// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Webhooks.Services;

public class WebhookDomainService : DomainService
{
    private readonly ILogger<WebhookDomainService> _logger;
    private readonly IWebhookRepository _webhookRepository;

    static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve,
    };

    public WebhookDomainService(ILogger<WebhookDomainService> logger, IWebhookRepository webhookRepository)
    {
        _logger = logger;
        _webhookRepository = webhookRepository;
    }

    public async Task TriggerAsync(WebhookEvent webhookEvent, object data)
    {
        var webhooks = await _webhookRepository.GetListAsync(w => w.WebhookEvent == webhookEvent && w.IsActive);
        using var client = new HttpClient();

        var payload = new
        {
            Event = webhookEvent.ToString(),
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Data = data
        };

        var request = new HttpRequestMessage();
        request.Content = new StringContent(JsonSerializer.Serialize(payload, _jsonSerializerOptions), Encoding.UTF8, "application/json");
        request.Method = HttpMethod.Post;

        foreach (var webhook in webhooks)
        {
            try
            {
                if (!string.IsNullOrEmpty(webhook.Secret))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", webhook.Secret);
                }
                request.RequestUri = new Uri(webhook.Url);

                var response = await client.SendAsync(request);
                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug($"Webhook triggered successfully.");
                }
                else
                {
                    _logger.LogWarning($"Failed to trigger webhook. Response status code: {response.StatusCode}");
                }
                webhook.AddLog(new WebhookLog(JsonSerializer.Serialize(data, _jsonSerializerOptions)));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Webhook request error");
            }
        }
    }
}
