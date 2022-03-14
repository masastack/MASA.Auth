namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class IdentityResourceClaim : UserClaim
{
    public int IdentityResourceId { get; set; }

    public IdentityResource IdentityResource { get; set; } = null!;
}

