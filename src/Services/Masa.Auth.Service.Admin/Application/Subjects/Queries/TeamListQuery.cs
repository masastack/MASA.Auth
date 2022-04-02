namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamListQuery(string Name) : Query<List<TeamDto>>
{
    public override List<TeamDto> Result { get; set; } = new();
}
