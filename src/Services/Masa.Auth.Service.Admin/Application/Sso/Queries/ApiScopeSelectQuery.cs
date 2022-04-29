namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiScopeSelectQuery(string Search) : Query<List<ApiScopeSelectDto>>
{
    public override List<ApiScopeSelectDto> Result { get; set; } = new();
}
