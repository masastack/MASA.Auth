namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record CustomLoginDetailQuery(int CustomLoginId) : Query<CustomLoginDetailDto>
{
    public override CustomLoginDetailDto Result { get; set; } = new();
}
