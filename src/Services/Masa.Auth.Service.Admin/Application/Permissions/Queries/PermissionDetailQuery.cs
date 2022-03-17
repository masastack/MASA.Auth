namespace Masa.Auth.Service.Admin.Application.Permissions.Queries;

public record PermissionDetailQuery(Guid PermissionId) : Query<PermissionDetailDto>
{
    public override PermissionDetailDto Result { get; set; } = new();
}
