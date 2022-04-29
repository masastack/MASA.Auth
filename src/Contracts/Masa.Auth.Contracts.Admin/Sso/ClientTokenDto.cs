namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientTokenDto
{
    public bool AllowOfflineAccess { get; set; }

    public int IdentityTokenLifetime { get; set; }

    public string AllowedIdentityTokenSigningAlgorithms
    {
        get
        {
            return string.Join(",", AllowTokenSigningAlgorithms);
        }
        set
        {
            AllowTokenSigningAlgorithms = value.Split(",").ToList();
        }
    }

    public List<string> AllowTokenSigningAlgorithms { get; set; } = new();

    public List<SelectItemDto<string>> AllowedIdentityTokenSigningAlgorithmsItems { get; set; } = new();

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
