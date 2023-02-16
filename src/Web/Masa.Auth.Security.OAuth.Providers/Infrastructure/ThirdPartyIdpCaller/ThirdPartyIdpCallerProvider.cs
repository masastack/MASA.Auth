// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers;

public static class ThirdPartyIdpCallerProvider
{
    readonly static List<ThirdPartyIdpCallerBase> _callers;

    static ThirdPartyIdpCallerProvider()
    {
        var callerTypes = Assembly.GetExecutingAssembly()
                             .GetTypes()
                             .Where(type => type.IsAbstract is false && type.IsAssignableTo(typeof(ThirdPartyIdpCallerBase)));

        _callers = callerTypes.Select(type => (ThirdPartyIdpCallerBase)type.Assembly.CreateInstance(type.FullName!)!).ToList();
    }

    public static async Task<Identity> GetIdentity(AuthenticationDefaults authenticationDefaults, string code)
    {
        var caller = _callers.FirstOrDefault(caller => caller.ThirdPartyIdpType == authenticationDefaults.ThirdPartyIdpType) ?? throw new UserFriendlyException($"Implementation without {authenticationDefaults.ThirdPartyIdpType}");
        var options = new OAuthOptions();
        authenticationDefaults.BindOAuthOptions(options);
        var tokenResponse = await caller.ExchangeCodeAsync(options, code);
        var principal = await caller.CreateTicketAsync(options, tokenResponse);
        return Identity.CreaterDefault(principal);
    }
}
