// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller;

public class JwtTokenValidator
{
    readonly JwtTokenValidatorOptions _jwtTokenValidatorOptions;
    readonly ClientRefreshTokenOptions _clientRefreshTokenOptions;
    readonly HttpClient _httpClient;
    readonly ILogger<JwtTokenValidator> _logger;

    public JwtTokenValidator(
        JwtTokenValidatorOptions jwtTokenValidatorOptions,
        HttpClient httpClient,
        ILogger<JwtTokenValidator> logger,
        ClientRefreshTokenOptions clientRefreshTokenOptions)
    {
        _jwtTokenValidatorOptions = jwtTokenValidatorOptions;
        _httpClient = httpClient;
        _logger = logger;
        _clientRefreshTokenOptions = clientRefreshTokenOptions;
    }

    public async Task<ClaimsPrincipal?> ValidateAccessTokenAsync(TokenProvider tokenProvider)
    {
        var disco = await _httpClient.GetDiscoveryDocumentAsync(_jwtTokenValidatorOptions.AuthorityEndpoint).ConfigureAwait(false);

        var keys = new List<SecurityKey>();
        foreach (var webKey in disco.KeySet.Keys)
        {
            var e = Base64Url.Decode(webKey.E);
            var n = Base64Url.Decode(webKey.N);
            var key = new RsaSecurityKey(new RSAParameters { Exponent = e, Modulus = n })
            {
                KeyId = webKey.Kid
            };
            keys.Add(key);
        }
        //var authorityEndpoint = "https://demo.identityserver.io/";
        //var openIdConfigurationEndpoint = $"{authorityEndpoint}.well-known/openid-configuration";
        //IConfigurationManager<OpenIdConnectConfiguration> configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(openIdConfigurationEndpoint, new OpenIdConnectConfigurationRetriever());
        //OpenIdConnectConfiguration openIdConfig = await configurationManager.GetConfigurationAsync(CancellationToken.None);
        TokenValidationParameters validationParameters = new TokenValidationParameters
        {
            ValidateLifetime = _jwtTokenValidatorOptions.ValidateLifetime,
            ValidateAudience = _jwtTokenValidatorOptions.ValidateAudience,
            ValidateIssuer = _jwtTokenValidatorOptions.ValidateIssuer,
            ValidIssuer = disco.Issuer,
            ValidAudiences = _jwtTokenValidatorOptions.ValidAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = keys
        };
        JwtSecurityTokenHandler handler = new();
        handler.InboundClaimTypeMap.Clear();
        ClaimsPrincipal? claimsPrincipal = null;
        try
        {
            claimsPrincipal = handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
        }
        catch (SecurityTokenExpiredException)
        {
            if (!string.IsNullOrEmpty(tokenProvider.RefreshToken))
            {
                var tokenClient = new TokenClient(_httpClient, new TokenClientOptions
                {
                    Address = disco.TokenEndpoint,
                    ClientId = _clientRefreshTokenOptions.ClientId,
                    ClientSecret = _clientRefreshTokenOptions.ClientSecret,
                    //Parameters =
                    //{
                    //    { "scope", "api1" }
                    //}
                });
                var tokenResult = await tokenClient.RequestRefreshTokenAsync(tokenProvider.RefreshToken).ConfigureAwait(false);
                if (tokenResult.IsError)
                {
                    _logger.LogError(tokenResult.Error);
                }
                else
                {
                    tokenProvider.AccessToken = tokenResult.AccessToken;
                    return handler.ValidateToken(tokenProvider.AccessToken, validationParameters, out SecurityToken _);
                }
            }
            else
            {
                _logger.LogWarning("RefreshToken is null,please AllowOfflineAccess value(true) and RequestedScopes should contains offline_access");
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e, "JwtTokenValidator failed");
        }
        return claimsPrincipal;
    }
}
