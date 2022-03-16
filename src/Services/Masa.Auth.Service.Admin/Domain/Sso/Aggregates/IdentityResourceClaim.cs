namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResourceClaim : UserClaim
{
    public int IdentityResourceId { get; }

    public IdentityResource IdentityResource { get; } = null!;
}

