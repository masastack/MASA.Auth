namespace Masa.Auth.Service.Application.Permissions.Queries;

public record PermissionDetailQuery(Guid PermissionId) : Query<PermissionDetail>
{
    public override PermissionDetail Result { get; set; } = new();
}
