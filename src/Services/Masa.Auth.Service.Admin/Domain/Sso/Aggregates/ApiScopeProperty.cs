using Masa.Auth.Service.Admin.Domain.Sso.Aggregates.Abstract;

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScopeProperty : Property
{
    public int ScopeId { get; set; }
    public ApiScope Scope { get; set; } = null!;
}
