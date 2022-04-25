namespace Masa.Auth.Service.Admin.Application.Projects.Queries;

public record ProjectListQuery(bool HasMenu) : Query<List<ProjectDto>>
{
    public override List<ProjectDto> Result { get; set; } = new();
}
