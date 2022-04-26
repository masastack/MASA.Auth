namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record IdentityResourceDetailQuery(int IdentityResourceId) : Query<IdentityResourceDetailDto>
{
    public override IdentityResourceDetailDto Result { get; set; } = new();
}
