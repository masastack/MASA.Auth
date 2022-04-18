namespace Masa.Auth.Service.Admin.Application.Projects.Queries;

public record AppTagsQuery : Query<List<string>>
{
    public override List<string> Result { get; set; } = new();
}
