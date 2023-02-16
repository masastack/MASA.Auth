// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

public class WeChatCaller : ThirdPartyIdpCallerBase
{
    public override ThirdPartyIdpTypes ThirdPartyIdpType { get; } = ThirdPartyIdpTypes.WeChat;

    public override async Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens)
    {
        var parameters = new Dictionary<string, string?>
        {
            ["access_token"] = tokens.AccessToken,
            ["openid"] = tokens.Response?.RootElement.GetString("openid"),
        };

        var address = QueryHelpers.AddQueryString(options.UserInformationEndpoint, parameters);

        using var response = await HttpClient.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            throw new UserFriendlyException("An error occurred while retrieving user information.");
        }

        var json = await response.Content.ReadAsStringAsync();
        using var payload = JsonDocument.Parse(json);

        var errorCode = payload.RootElement.GetString("errcode");
        if (!string.IsNullOrEmpty(errorCode))
        {
            throw new UserFriendlyException("An error occurred while retrieving user information.");
        }

        var identity = new ClaimsIdentity();
        (var openId, var unionId) =  (payload.RootElement.GetString("openid"), payload.RootElement.GetString("unionid"));
        var nameIdentifier = !string.IsNullOrWhiteSpace(unionId) ? unionId : openId ?? throw new UserFriendlyException("Get weChat identifier failed.");
        identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, nameIdentifier, ClaimValueTypes.String, options.ClaimsIssuer));     

        var principal = new ClaimsPrincipal(identity);
        foreach (var action in options.ClaimActions)
        {
            action.Run(payload.RootElement, identity, options.ClaimsIssuer ?? ThirdPartyIdpType.ToString());
        }

        return principal;
    }

    public override async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code)
    {
        var tokenRequestParameters = new Dictionary<string, string?>()
        {
            ["appid"] = options.ClientId,
            ["secret"] = options.ClientSecret,
            ["code"] = code,
            ["grant_type"] = "authorization_code",
        };

        var address = QueryHelpers.AddQueryString(options.TokenEndpoint, tokenRequestParameters);

        using var response = await HttpClient.GetAsync(address);
        if (!response.IsSuccessStatusCode)
        {
            throw new UserFriendlyException("An error occurred while retrieving an access token.");
        }

        var json = await response.Content.ReadAsStringAsync();
        var payload = JsonDocument.Parse(json);

        var errorCode = payload.RootElement.GetString("errcode");
        if (!string.IsNullOrEmpty(errorCode))
        {
            throw new UserFriendlyException("An error occurred while retrieving an access token.");
        }

        return OAuthTokenResponse.Success(payload);
    }
}
