namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiScopeProperty : Property
{
    public int ScopeId { get; set; }
    public ApiScope Scope { get; set; } = null!;
}
