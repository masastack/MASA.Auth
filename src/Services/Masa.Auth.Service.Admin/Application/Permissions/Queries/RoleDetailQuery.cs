namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleDetailQuery(Guid RoleId) : Query<RoleDetailDto>
{
    public override RoleDetailDto Result { get; set; } = null!;
}
