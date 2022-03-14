namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiScopeClaim : UserClaim
{
    public int ScopeId { get; set; }

    public ApiScope Scope { get; set; } = null!;
}
