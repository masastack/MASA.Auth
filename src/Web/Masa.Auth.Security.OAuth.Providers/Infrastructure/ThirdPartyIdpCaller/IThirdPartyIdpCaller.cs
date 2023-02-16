// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpApiHelper;

public interface IThirdPartyIdpCaller
{
    ThirdPartyIdpTypes ThirdPartyIdpType { get; }

    Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code);

    Task<JsonDocument> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens);
}
