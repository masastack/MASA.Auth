namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class Client : AggregateRoot<int>
{
    public bool Enabled { get; } = true;

    public string ClientId { get; } = string.Empty;

    public string ProtocolType { get; } = "oidc";

    public List<ClientSecret> ClientSecrets { get; } = new();

    public bool RequireClientSecret { get; } = true;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; } = string.Empty;

    public string ClientUri { get; } = string.Empty;

    public string LogoUri { get; } = string.Empty;

    public bool RequireConsent { get; } = false;

    public bool AllowRememberConsent { get; } = true;

    public bool AlwaysIncludeUserClaimsInIdToken { get; }

    public List<ClientGrantType> AllowedGrantTypes { get; } = new();

    public bool RequirePkce { get; } = true;

    public bool AllowPlainTextPkce { get; }

    public bool RequireRequestObject { get; }

    public bool AllowAccessTokensViaBrowser { get; }

    public List<ClientRedirectUri> RedirectUris { get; } = new();

    public List<ClientPostLogoutRedirectUri> PostLogoutRedirectUris { get; } = new();

    public string FrontChannelLogoutUri { get; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; } = true;

    public string BackChannelLogoutUri { get; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; } = true;

    public bool AllowOfflineAccess { get; }

    public List<ClientScope> AllowedScopes { get; } = new();

    public int IdentityTokenLifetime { get; } = 300;

    public string AllowedIdentityTokenSigningAlgorithms { get; } = string.Empty;

    public int AccessTokenLifetime { get; } = 3600;

    public int AuthorizationCodeLifetime { get; } = 300;

    public int? ConsentLifetime { get; } = null;

    public int AbsoluteRefreshTokenLifetime { get; } = 2592000;

    public int SlidingRefreshTokenLifetime { get; } = 1296000;

    public int RefreshTokenUsage { get; } = (int)TokenUsage.OneTimeOnly;

    public bool UpdateAccessTokenClaimsOnRefresh { get; }

    public int RefreshTokenExpiration { get; } = (int)TokenExpiration.Absolute;

    public int AccessTokenType { get; } = (int)0; // AccessTokenType.Jwt;

    public bool EnableLocalLogin { get; } = true;

    public List<ClientIdPRestriction> IdentityProviderRestrictions { get; } = new();

    public bool IncludeJwtId { get; }

    public List<ClientClaim> Claims { get; } = new();

    public bool AlwaysSendClientClaims { get; }

    public string ClientClaimsPrefix { get; } = "client_";

    public string PairWiseSubjectSalt { get; } = string.Empty;

    public List<ClientCorsOrigin> AllowedCorsOrigins { get; } = new();

    public List<ClientProperty> Properties { get; } = new();

    public DateTime Created { get; } = DateTime.UtcNow;

    public DateTime? Updated { get; }

    public DateTime? LastAccessed { get; }

    public int? UserSsoLifetime { get; }

    public string UserCodeType { get; } = string.Empty;

    public int DeviceCodeLifetime { get; } = 300;

    public bool NonEditable { get; }
}

