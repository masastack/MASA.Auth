// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

public abstract class ThirdPartyIdpCallerBase
{
    static HttpClient? _httpClient;

    protected static HttpClient HttpClient = _httpClient ??= new HttpClient();

    public abstract ThirdPartyIdpTypes ThirdPartyIdpType { get; }

    public abstract Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code);

    public abstract Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens);
}
