namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record IdentityResourceSelectQuery(string Search) : Query<List<IdentityResourceSelectDto>>
{
    public override List<IdentityResourceSelectDto> Result { get; set; } = new();
}
