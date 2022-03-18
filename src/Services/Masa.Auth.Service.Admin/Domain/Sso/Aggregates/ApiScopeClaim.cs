namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeClaim : UserClaim
{
    public int ScopeId { get; private set; }

    public ApiScope Scope { get; private set; } = null!;
}
