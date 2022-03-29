namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamSelectListQuery : Query<List<TeamSelectDto>>
{
    public override List<TeamSelectDto> Result { get; set; } = new();
}
