namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiScopeDetailQuery(int ApiScopeId) : Query<ApiScopeDetailDto>
{
    public override ApiScopeDetailDto Result { get; set; } = new();
}
