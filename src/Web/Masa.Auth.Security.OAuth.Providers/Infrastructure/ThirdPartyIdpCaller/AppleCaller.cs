// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

public class AppleCaller : ThirdPartyIdpCallerBase
{
    readonly ILogger<AppleCaller> _logger;
    readonly HttpClient _httpClient;
    readonly AppleAuthenticationOptions _options;

    public AppleCaller(ILogger<AppleCaller> logger, IHttpClientFactory httpClientFactory, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _options = ((CustomOptionsMonitor<AppleAuthenticationOptions>)ActivatorUtilities.CreateInstance(serviceProvider, typeof(CustomOptionsMonitor<AppleAuthenticationOptions>))).CurrentValue;
    }

    public ThirdPartyIdpTypes ThirdPartyIdpType { get; } = ThirdPartyIdpTypes.Apple;

    public async Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens)
    {
        string? idToken = tokens.Response!.RootElement.GetString("id_token");
        if (string.IsNullOrWhiteSpace(idToken))
        {
            throw new UserFriendlyException("No Apple ID token was returned in the OAuth token response.");
        }
        var configuration = await _options.ConfigurationManager!.GetConfigurationAsync(default);
        var validationParameters = _options.TokenValidationParameters.Clone();
        validationParameters.ValidateAudience = false;
        validationParameters.IssuerSigningKeys = configuration.JsonWebKeySet.Keys;
        try
        {
            var result = _options.SecurityTokenHandler.ValidateToken(idToken, validationParameters);

            if (result.Exception is not null || !result.IsValid)
            {
                throw new UserFriendlyException("Apple ID token validation failed.", result.Exception);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, idToken);
            _logger.LogError(ex, idToken);
            throw;
        }
        var securityToken = _options.SecurityTokenHandler.ReadJsonWebToken(idToken);
        var identity = new ClaimsIdentity(securityToken.Claims);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(identity));
        foreach (var action in options.ClaimActions)
        {
            if(action is JsonKeyClaimAction jkAction)
            {
                var claim = identity.FindFirst(jkAction.JsonKey);
                if (!string.IsNullOrEmpty(claim?.Value))
                {
                    identity.AddClaim(new Claim(action.ClaimType, claim.Value, claim.ValueType, options.ClaimsIssuer));
                }
            }
        }

        return principal;
    }

    public async Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code)
    {
        throw new NotImplementedException();
    }
}
