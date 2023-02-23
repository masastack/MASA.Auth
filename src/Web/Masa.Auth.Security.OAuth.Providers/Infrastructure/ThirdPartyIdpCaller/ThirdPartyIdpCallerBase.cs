// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Security.OAuth.Providers.Infrastructure.ThirdPartyIdpCaller;

public interface ThirdPartyIdpCallerBase : ISingletonDependency
{
    ThirdPartyIdpTypes ThirdPartyIdpType { get; }

    Task<OAuthTokenResponse> ExchangeCodeAsync(OAuthOptions options, string code);

    Task<ClaimsPrincipal> CreateTicketAsync(OAuthOptions options, OAuthTokenResponse tokens);
}
