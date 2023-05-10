// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public class ThirdPartyIdpCallerProvider : ISingletonDependency
{
    readonly IEnumerable<ThirdPartyIdpCallerBase> _callers;

    public ThirdPartyIdpCallerProvider(IEnumerable<ThirdPartyIdpCallerBase> callers)
    {
        _callers = callers;
    }

    public async Task<Identity> GetIdentity(AuthenticationDefaults authenticationDefaults, string code)
    {
        var caller = _callers.FirstOrDefault(caller => caller.ThirdPartyIdpType == authenticationDefaults.ThirdPartyIdpType) ?? throw new UserFriendlyException($"Implementation without {authenticationDefaults.ThirdPartyIdpType}");
        var options = new OAuthOptions();
        authenticationDefaults.BindOAuthOptions(options);
        var tokenResponse = await caller.ExchangeCodeAsync(options, code);
        var principal = await caller.CreateTicketAsync(options, tokenResponse);
        return Identity.CreaterDefault(principal);
    }

    public async Task<Identity> GetIdentityByIdToken(AuthenticationDefaults authenticationDefaults, string idToken)
    {
        var caller = _callers.FirstOrDefault(caller => caller.ThirdPartyIdpType == authenticationDefaults.ThirdPartyIdpType) ?? throw new UserFriendlyException($"Implementation without {authenticationDefaults.ThirdPartyIdpType}");
        var options = new OAuthOptions();
        authenticationDefaults.BindOAuthOptions(options);
        var tokenResponse = OAuthTokenResponse.Success(JsonDocument.Parse("{\"id_token\":\"" + idToken + "\"}")); ;
        var principal = await caller.CreateTicketAsync(options, tokenResponse);
        return Identity.CreaterDefault(principal);
    }
}
