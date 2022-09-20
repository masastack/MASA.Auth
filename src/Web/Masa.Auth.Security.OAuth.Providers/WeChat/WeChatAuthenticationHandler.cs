// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.WeChat;

public class WeChatAuthenticationHandler : WeixinAuthenticationHandler
{
    private readonly string _oauthState = "_oauthstate";
    private readonly string _state = "state";

    public WeChatAuthenticationHandler(
       IOptionsMonitor<WeixinAuthenticationOptions> options,
       ILoggerFactory logger,
       UrlEncoder encoder,
       ISystemClock clock)
       : base(options, logger, encoder, clock)
    {
    }

    protected override string BuildChallengeUrl(AuthenticationProperties properties, string redirectUri)
    {
        var scopeParameter = properties.GetParameter<ICollection<string>>(OAuthChallengeProperties.ScopeKey);
        var scope = scopeParameter != null ? FormatScope(scopeParameter) : FormatScope();

        var parameters = new Dictionary<string, string?>()
        {
            ["appid"] = Options.ClientId,
            ["scope"] = scope,
            ["response_type"] = "code",
        };

        if (Options.UsePkce)
        {
            var bytes = RandomNumberGenerator.GetBytes(256 / 8);
            var codeVerifier = WebEncoders.Base64UrlEncode(bytes);

            // Store this for use during the code redemption.
            properties.Items.Add(OAuthConstants.CodeVerifierKey, codeVerifier);

            var challengeBytes = SHA256.HashData(Encoding.UTF8.GetBytes(codeVerifier));

            parameters[OAuthConstants.CodeChallengeKey] = WebEncoders.Base64UrlEncode(challengeBytes);
            parameters[OAuthConstants.CodeChallengeMethodKey] = OAuthConstants.CodeChallengeMethodS256;
        }

        var state = Options.StateDataFormat.Protect(properties);
        var addRedirectHash = false;

        if (IsWeixinAuthorizationEndpointInUse())
        {
            // Store state in redirectUri when authorizing Wechat Web pages to prevent "too long state parameters" error
            redirectUri = QueryHelpers.AddQueryString(redirectUri, _oauthState, state);
            addRedirectHash = true;
        }

        parameters["redirect_uri"] = redirectUri;
        parameters[_state] = addRedirectHash ? _oauthState : state;

        var challengeUrl = QueryHelpers.AddQueryString(Options.AuthorizationEndpoint, parameters);

        if (addRedirectHash)
        {
            // The parameters necessary for Web Authorization of Wechat
            challengeUrl += "#wechat_redirect";
        }

        return challengeUrl;
    }

    private bool IsWeixinAuthorizationEndpointInUse()
    {
        return string.Equals(Options.AuthorizationEndpoint, WeixinAuthenticationDefaults.AuthorizationEndpoint, StringComparison.OrdinalIgnoreCase);
    }
}
