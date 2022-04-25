namespace Masa.Auth.Contracts.Admin.Sso;

internal class ClientDetailDto : ClientAddDto
{
    #region Basic
    public bool Enabled { get; set; }

    public bool RequireRequestObject { get; set; }

    public bool AllowOfflineAccess { get; set; }

    public bool RequireClientSecret { get; set; }
    #endregion

    #region Authentication
    public string FrontChannelLogoutUri { get; set; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool EnableLocalLogin { get; set; }

    public List<string> IdentityProviderRestrictions { get; set; } = new();

    public int UserSsoLifetime { get; set; }
    #endregion

    #region Token
    public int IdentityTokenLifetime { get; set; }

    public int AccessTokenLifetime { get; set; }

    public int AuthorizationCodeLifetime { get; set; }

    public int AbsoluteRefreshTokenLifetime { get; set; }

    public bool IncludeJwtId { get; set; }

    public int SlidingRefreshTokenLifetime { get; set; }

    public int RefreshTokenUsage { get; set; }

    public int RefreshTokenExpiration { get; set; }

    public bool UpdateAccessTokenClaimsOnRefresh { get; set; }

    public List<string> AllowedCorsOrigins { get; set; } = new();

    public bool AlwaysSendClientClaims { get; set; }

    public string PairWiseSubjectSalt { get; set; } = string.Empty;
    #endregion

    #region Consent Screen
    public bool AllowRememberConsent { get; set; }

    public int ConsentLifetime { get; set; }
    #endregion

    #region ClientSecret
    public List<ClientSecretDto> ClientSecrets { get; set; } = new();
    #endregion

    #region IdentityResource
    #endregion

    #region ApiResource
    #endregion
}
