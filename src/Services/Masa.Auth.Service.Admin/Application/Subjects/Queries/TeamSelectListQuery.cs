namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamSelectListQuery(string Name) : Query<List<TeamSelectDto>>
{
    public override List<TeamSelectDto> Result { get; set; } = new();
}
