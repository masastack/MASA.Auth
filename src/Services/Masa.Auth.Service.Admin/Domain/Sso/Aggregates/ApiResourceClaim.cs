namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResourceClaim : Entity<int>
{
    public int UserClaimId { get; private set; }

    public UserClaim UserClaim { get; private set; } = null!;

    public int ApiResourceId { get; private set; }

    public ApiResource ApiResource { get; private set; } = null!;

    public ApiResourceClaim(int userClaimId)
    {
        UserClaimId = userClaimId;
    }
}

