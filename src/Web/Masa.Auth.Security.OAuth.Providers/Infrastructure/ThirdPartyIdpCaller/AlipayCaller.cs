// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

/// <summary>
/// 支付宝第三方身份提供商调用器
/// 按照开放平台文档通过网关签名调用：
/// - 获取令牌：method=alipay.system.oauth.token
/// - 获取用户：method=alipay.user.info.share
/// 文档：
/// - 接入指南：https://opendocs.alipay.com/open/01emu5?pathHash=8f9c00bc
/// - 换取授权访问令牌接口：https://opendocs.alipay.com/open/02ahjv?pathHash=aca75fe9
/// - 支付宝会员授权信息查询接口：https://opendocs.alipay.com/open/02ailg?pathHash=032278f2
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
        var parameters = new SortedDictionary<string, string?>(StringComparer.Ordinal)
        {
            ["app_id"] = options.ClientId,
            ["charset"] = "utf-8",
            ["code"] = code,
            ["format"] = "JSON",
            ["grant_type"] = "authorization_code",
            ["method"] = "alipay.system.oauth.token",
            ["sign_type"] = "RSA2",
            ["timestamp"] = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            ["version"] = "1.0",
        };
        parameters.Add("sign", GetRSA2Signature(parameters, options.ClientSecret));

        var address = QueryHelpers.AddQueryString(options.TokenEndpoint, parameters);

        using var response = await _httpClient.GetAsync(address);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Alipay token request failed: {StatusCode}, {Body}", response.StatusCode, json);
            throw new UserFriendlyException("An error occurred while retrieving an access token.");
        }

        using var payload = JsonDocument.Parse(json);
        if (payload.RootElement.TryGetProperty("error_response", out var error))
        {
            var codeStr = error.TryGetProperty("code", out var ec) ? ec.GetString() : null;
            var msg = error.TryGetProperty("msg", out var em) ? em.GetString() : null;
            var sub = error.TryGetProperty("sub_msg", out var sm) ? sm.GetString() : null;
            _logger.LogError("Alipay token error: code={Code}, msg={Msg}, sub_msg={Sub}", codeStr, msg, sub);
            throw new UserFriendlyException($"Alipay token error: {msg} {sub}".Trim());
        }

        if (!payload.RootElement.TryGetProperty("alipay_system_oauth_token_response", out var tokenNode))
        {
            _logger.LogError("Unexpected token response: {Body}", json);
            throw new UserFriendlyException("Unexpected Alipay token response");
        }

        var tokenJson = JsonSerializer.Serialize(tokenNode);
        var tokenDoc = JsonDocument.Parse(tokenJson);
        return OAuthTokenResponse.Success(tokenDoc);
    }

    /// <summary>
    /// 使用访问令牌获取用户信息并创建票据
    /// </summary>
    public async Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens)
    {
        var accessToken = tokens.AccessToken;
        if (string.IsNullOrWhiteSpace(accessToken))
            throw new UserFriendlyException("Alipay access token is empty");

        var parameters = new SortedDictionary<string, string?>(StringComparer.Ordinal)
        {
            ["app_id"] = options.ClientId,
            ["auth_token"] = tokens.AccessToken,
            ["charset"] = "utf-8",
            ["format"] = "JSON",
            ["method"] = "alipay.user.info.share",
            ["sign_type"] = "RSA2",
            ["timestamp"] = DateTimeOffset.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            ["version"] = "1.0",
        };
        parameters.Add("sign", GetRSA2Signature(parameters, options.ClientSecret));

        var address = QueryHelpers.AddQueryString(options.UserInformationEndpoint, parameters);

        using var response = await _httpClient.GetAsync(address);
        var json = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("Alipay userinfo request failed: {StatusCode}, {Body}", response.StatusCode, json);
            throw new UserFriendlyException("An error occurred while retrieving user information.");
        }

        using var payload = JsonDocument.Parse(json);
        if (payload.RootElement.TryGetProperty("error_response", out var error))
        {
            var codeStr = error.TryGetProperty("code", out var ec) ? ec.GetString() : null;
            var msg = error.TryGetProperty("msg", out var em) ? em.GetString() : null;
            var sub = error.TryGetProperty("sub_msg", out var sm) ? sm.GetString() : null;
            _logger.LogError("Alipay userinfo error: code={Code}, msg={Msg}, sub_msg={Sub}", codeStr, msg, sub);
            throw new UserFriendlyException($"Alipay userinfo error: {msg} {sub}".Trim());
        }

        if (!payload.RootElement.TryGetProperty("alipay_user_info_share_response", out var userNode))
        {
            _logger.LogError("Unexpected userinfo response: {Body}", json);
            throw new UserFriendlyException("Unexpected Alipay userinfo response");
        }

        var identity = new ClaimsIdentity();
        // 优先使用open_id，如果没有则使用user_id
        var userId = userNode.TryGetProperty("open_id", out var oid) ? oid.GetString() : null;
        if (string.IsNullOrWhiteSpace(userId))
            userId = userNode.TryGetProperty("user_id", out var uid) ? uid.GetString() : null;
        if (string.IsNullOrWhiteSpace(userId))
            throw new UserFriendlyException("Get Alipay user identifier failed.");

        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, userId!, ClaimValueTypes.String, options.ClaimsIssuer));
        var principal = new ClaimsPrincipal(identity);

        foreach (var action in options.ClaimActions)
        {
            action.Run(userNode, identity, options.ClaimsIssuer ?? ThirdPartyIdpType.ToString());
        }

        return principal;
    }

    /// <summary>
    /// Gets the RSA2(SHA256 with RSA) signature.
    /// </summary>
    /// <param name="sortedPairs">Sorted key-value pairs</param>
    private string GetRSA2Signature([NotNull] SortedDictionary<string, string?> sortedPairs, string clientSecret)
    {
        var builder = new StringBuilder(128);

        foreach (var pair in sortedPairs)
        {
            if (string.IsNullOrEmpty(pair.Value))
            {
                continue;
            }

            builder.Append(pair.Key)
                   .Append('=')
                   .Append(pair.Value)
                   .Append('&');
        }

        var plainText = builder.ToString();
        var plainBytes = Encoding.UTF8.GetBytes(plainText, 0, plainText.Length - 1); // Skip the last '&'
        var privateKeyBytes = Convert.FromBase64String(clientSecret);

        using var rsa = RSA.Create();
        rsa.ImportPkcs8PrivateKey(privateKeyBytes, out _);

        var encryptedBytes = rsa.SignData(plainBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return Convert.ToBase64String(encryptedBytes);
    }
}
