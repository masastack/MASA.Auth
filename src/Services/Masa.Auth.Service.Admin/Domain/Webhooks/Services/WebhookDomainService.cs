// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Webhooks.Services;

public class WebhookDomainService : DomainService
{
    private readonly ILogger<WebhookDomainService> _logger;
    private readonly IWebhookRepository _webhookRepository;
    private readonly IHttpClientFactory _httpClientFactory;

    static readonly JsonSerializerOptions _jsonSerializerOptions = new JsonSerializerOptions
    {
        ReferenceHandler = ReferenceHandler.Preserve,
    };

    public WebhookDomainService(ILogger<WebhookDomainService> logger, IWebhookRepository webhookRepository, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _webhookRepository = webhookRepository;
        _httpClientFactory = httpClientFactory;
    }

    public async Task TriggerAsync(WebhookEvent webhookEvent, object data)
    {
        var webhooks = await _webhookRepository.GetListAsync(w => w.Event == webhookEvent && w.IsActive);
        if (!webhooks.Any())
            return;

        var payload = new
        {
            Event = webhookEvent.ToString(),
            Timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Data = data
        };

        //每个 webhook 独立构造请求:复用同一个 HttpRequestMessage 会累积前一个的
        //Authorization 头,导致后续接收方看到重复鉴权头
        var client = _httpClientFactory.CreateClient("Webhook");
        foreach (var webhook in webhooks)
        {
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, webhook.Url);
                request.Content = new StringContent(JsonSerializer.Serialize(payload, _jsonSerializerOptions), Encoding.UTF8, "application/json");
                if (!string.IsNullOrEmpty(webhook.Secret))
                {
                    request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", webhook.Secret);
                }

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
