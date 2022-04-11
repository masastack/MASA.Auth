namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record MenuPermissionDetailQuery(Guid PermissionId) : Query<MenuPermissionDetailDto>
{
    public override MenuPermissionDetailDto Result { get; set; } = new();
}
