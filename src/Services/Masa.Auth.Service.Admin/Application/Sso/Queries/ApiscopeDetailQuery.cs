namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiscopeDetailQuery(int ApiscopeId) : Query<ApiScopeDetailDto>
{
    public override ApiScopeDetailDto Result { get; set; } = new();
}
