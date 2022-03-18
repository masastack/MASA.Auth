namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeProperty : Property
{
    public int ScopeId { get; private set; }

    public ApiScope Scope { get; private set; } = null!;
}
