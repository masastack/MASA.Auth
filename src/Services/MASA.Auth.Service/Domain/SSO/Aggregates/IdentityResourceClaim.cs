namespace Masa.Auth.Service.Domain.SSO.Aggregates.IdentityResources;

public class IdentityResourceClaim : UserClaim
{
    public int IdentityResourceId { get; set; }
    public IdentityResource IdentityResource { get; set; } = null!;
}

