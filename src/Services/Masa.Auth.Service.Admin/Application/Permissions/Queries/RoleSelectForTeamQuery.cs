namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleSelectForTeamQuery(Guid TeamId,TeamMemberTypes TeamMemberType) : Query<List<RoleSelectDto>>
{
    public override List<RoleSelectDto> Result { get; set; } = new();
}
