using Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeClaim : UserClaim
{
    public int ScopeId { get; }

    public ApiScope Scope { get; } = null!;
}
