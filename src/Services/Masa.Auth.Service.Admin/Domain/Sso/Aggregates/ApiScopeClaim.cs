namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeClaim : Entity<int>
{
    public int UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public int ApiScopeId { get; private set; }

    public ApiScope ApiScope { get; private set; } = null!;

    public ApiScopeClaim(int userClaimId)
    {
        UserClaimId = userClaimId;
    }
}
