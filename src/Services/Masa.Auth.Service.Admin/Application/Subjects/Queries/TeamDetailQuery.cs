namespace Masa.Auth.Service.Admin.Application.Subjects.Queries;

public record TeamDetailQuery(Guid TeamId) : Query<TeamDetailDto>
{
    public override TeamDetailDto Result { get; set; } = new();
}
