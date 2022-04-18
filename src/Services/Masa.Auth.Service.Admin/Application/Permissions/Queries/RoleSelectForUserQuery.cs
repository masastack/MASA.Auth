namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleSelectForUserQuery(Guid UserId) : Query<List<RoleSelectDto>>
{
    public override List<RoleSelectDto> Result { get; set; } = new();
}
