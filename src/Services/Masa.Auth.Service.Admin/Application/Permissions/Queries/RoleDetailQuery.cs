namespace Masa.Auth.Service.Application.Permissions.Queries;

public record RoleDetailQuery(Guid RoleId) : Query<RoleDetail>
{
    public override RoleDetail Result { get; set; } = new();
}
