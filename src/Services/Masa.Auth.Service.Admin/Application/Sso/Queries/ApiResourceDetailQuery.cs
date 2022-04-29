namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ApiResourceDetailQuery(int ApiResourceId) : Query<ApiResourceDetailDto>
{
    public override ApiResourceDetailDto Result { get; set; } = new();
}
