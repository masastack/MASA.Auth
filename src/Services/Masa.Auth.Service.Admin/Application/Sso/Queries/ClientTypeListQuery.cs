namespace Masa.Auth.Service.Admin.Application.Sso.Queries;

public record ClientTypeListQuery : Query<List<ClientTypeDetailDto>>
{
    public override List<ClientTypeDetailDto> Result { get; set; } = new();
}
