namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record RoleOwnerQuery(Guid RoleId) : Query<RoleOwnerDto>
{
    public override RoleOwnerDto Result { get; set; } = null!;
}
