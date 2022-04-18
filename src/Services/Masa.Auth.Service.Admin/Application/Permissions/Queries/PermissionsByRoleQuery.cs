namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionsByRoleQuery(List<Guid> Roles) : Query<List<Guid>>
{
    public override List<Guid> Result { get; set; } = new();
}
