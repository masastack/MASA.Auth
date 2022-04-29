// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientTokenDto
{
    public bool AllowOfflineAccess { get; set; }

    public int IdentityTokenLifetime { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int AccessTokenType { get; set; }

    public int AuthorizationCodeLifetime { get; set; }

    public int AbsoluteRefreshTokenLifetime { get; set; }

    public bool IncludeJwtId { get; set; }

    public int SlidingRefreshTokenLifetime { get; set; }

    public int RefreshTokenUsage { get; set; }

    public int RefreshTokenExpiration { get; set; }

    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    public bool AlwaysSendClientClaims { get; set; }

    public string PairWiseSubjectSalt { get; set; } = string.Empty;

    public bool AllowAccessTokensViaBrowser { get; set; }

    public bool AlwaysIncludeUserClaimsInIdToken { get; set; }

    public List<ClientClaimDto> Claims { get; set; } = new();
}
