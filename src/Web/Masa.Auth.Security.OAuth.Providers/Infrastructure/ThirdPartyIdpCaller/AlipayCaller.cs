// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

/// <summary>
/// 支付宝第三方身份提供商调用器
/// </summary>
public class AlipayCaller : ThirdPartyIdpCallerBase
{
    readonly ILogger<AlipayCaller> _logger;
    readonly HttpClient _httpClient;

    public AlipayCaller(ILogger<AlipayCaller> logger, IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
    }

    public ThirdPartyIdpTypes ThirdPartyIdpType { get; } = ThirdPartyIdpTypes.Alipay;

    /// <summary>
    /// 使用授权码获取访问令牌
    /// </summary>
    public async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code)
    {
        var tokenRequestParameters = new Dictionary<string, string?>()
        {
            ["grant_type"] = "authorization_code",
            ["app_id"] = options.ClientId,
            ["code"] = code,
        };

        var address = QueryHelpers.AddQueryString(options.TokenEndpoint, tokenRequestParameters);

        using var response = await _httpClient.GetAsync(address);
        var json = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("支付宝获取访问令牌失败: {Json}", json);
            throw new UserFriendlyException("获取支付宝访问令牌时发生错误。");
        }

        var payload = JsonDocument.Parse(json);
        
        // 检查支付宝响应中的错误
        if (payload.RootElement.TryGetProperty("error_response", out var errorResponse))
        {
            var errorMsg = errorResponse.GetProperty("sub_msg").GetString() ?? "未知错误";
            _logger.LogError("支付宝授权错误: {ErrorMsg}, 完整响应: {Json}", errorMsg, json);
            throw new UserFriendlyException($"支付宝授权失败: {errorMsg}");
        }

        return OAuthTokenResponse.Success(payload);
    }

    /// <summary>
    /// 使用访问令牌获取用户信息并创建票据
    /// </summary>
    public async Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens)
    {
        var accessToken = tokens.AccessToken;
        if (string.IsNullOrEmpty(accessToken))
        {
            throw new UserFriendlyException("支付宝访问令牌为空。");
        }

        var parameters = new Dictionary<string, string?>
        {
            ["app_id"] = options.ClientId,
            ["method"] = "alipay.user.info.share",
            ["format"] = "JSON",
            ["charset"] = "UTF-8",
            ["sign_type"] = "RSA2",
            ["timestamp"] = DateTimeOffset.UtcNow.ToString("yyyy-MM-dd HH:mm:ss"),
            ["version"] = "1.0",
            ["auth_token"] = accessToken,
        };

        var address = QueryHelpers.AddQueryString(options.UserInformationEndpoint, parameters);

        using var response = await _httpClient.GetAsync(address);
        var json = await response.Content.ReadAsStringAsync();
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("获取支付宝用户信息失败: {Json}", json);
            throw new UserFriendlyException("获取支付宝用户信息时发生错误。");
        }

        using var payload = JsonDocument.Parse(json);
        
        // 检查支付宝API响应错误
        if (payload.RootElement.TryGetProperty("error_response", out var errorResponse))
        {
            var errorMsg = errorResponse.GetProperty("sub_msg").GetString() ?? "未知错误";
            _logger.LogError("获取支付宝用户信息错误: {ErrorMsg}, 完整响应: {Json}", errorMsg, json);
            throw new UserFriendlyException($"获取支付宝用户信息失败: {errorMsg}");
        }

        // 获取用户信息响应
        if (!payload.RootElement.TryGetProperty("alipay_user_info_share_response", out var userInfo))
        {
            _logger.LogError("支付宝用户信息响应格式异常: {Json}", json);
            throw new UserFriendlyException("支付宝用户信息响应格式异常。");
        }

        var identity = new ClaimsIdentity();
        var userId = userInfo.GetProperty("user_id").GetString();
        if (string.IsNullOrEmpty(userId))
        {
            throw new UserFriendlyException("获取支付宝用户标识失败。");
        }

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId, ClaimValueTypes.String, options.ClaimsIssuer));

        var principal = new ClaimsPrincipal(identity);
        
        // 应用用户声明映射
        foreach (var action in options.ClaimActions)
        {
            action.Run(userInfo, identity, options.ClaimsIssuer ?? ThirdPartyIdpType.ToString());
        }

        return principal;
    }
}
