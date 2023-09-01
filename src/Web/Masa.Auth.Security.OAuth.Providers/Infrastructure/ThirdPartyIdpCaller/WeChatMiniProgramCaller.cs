// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

public class WeChatMiniProgramCaller : ISingletonDependency
{
    readonly ILogger<WeChatMiniProgramCaller> _logger;
    readonly HttpClient _httpClient;

    public WeChatMiniProgramCaller(ILogger<WeChatMiniProgramCaller> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<string> GetOpenIdAsync(OAuthOptions options, string code)
    {
        var tokenRequestParameters = new Dictionary<string, string?>()
        {
            ["appid"] = options.ClientId,
            ["secret"] = options.ClientSecret,
            ["js_code"] = code,
            ["grant_type"] = "authorization_code",
        };

        var address = QueryHelpers.AddQueryString(options.TokenEndpoint, tokenRequestParameters);
        using var response = await _httpClient.GetAsync(address);
        var json = await response.Content.ReadAsStringAsync();
        var payload = JsonDocument.Parse(json);
        var errorCode = payload.RootElement.GetString("errcode");
        if (!string.IsNullOrEmpty(errorCode))
        {
            _logger.LogError(json);
            throw new UserFriendlyException($"An error occurred while get WeChat Mini Program openid.");
        }

        return payload.RootElement.GetString("openid") ?? throw new UserFriendlyException($"Get WeChat Mini Program openid failed.");
    }
}
