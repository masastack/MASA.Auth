// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientDetailDto : AddClientDto
{
    #region Basic
    public Guid Id { get; set; }

    public bool Enabled { get; set; }

    public bool RequireRequestObject { get; set; }

    public List<string> AllowedCorsOrigins { get; set; } = new();
    #endregion

    #region Authentication
    public string FrontChannelLogoutUri { get; set; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool EnableLocalLogin { get; set; }

    public List<string> IdentityProviderRestrictions { get; set; } = new();

    public int? UserSsoLifetime { get; set; }
    #endregion

    #region Token
    public bool AllowOfflineAccess { get; set; }

    public int IdentityTokenLifetime { get; set; }

    public string AllowedIdentityTokenSigningAlgorithms { get; set; } = string.Empty;

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
    #endregion

    #region Device Flow
    public string UserCodeType { get; set; } = string.Empty;

    public int DeviceCodeLifetime { get; set; } = 300;
    #endregion

    #region Client Credentials
    public List<ClientSecretDto> ClientSecrets { get; set; } = new();
    #endregion

    #region Consent Screen
    public bool AllowRememberConsent { get; set; }

    public int? ConsentLifetime { get; set; }
    #endregion

    #region Properties
    public List<ClientPropertyDto> Properties { get; set; } = new();
    #endregion
}
