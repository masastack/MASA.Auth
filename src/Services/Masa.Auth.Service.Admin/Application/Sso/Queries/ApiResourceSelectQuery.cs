namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiResourceSelectQuery(string Search) : Query<List<ApiResourceSelectDto>>
{
    public override List<ApiResourceSelectDto> Result { get; set; } = new();
}
