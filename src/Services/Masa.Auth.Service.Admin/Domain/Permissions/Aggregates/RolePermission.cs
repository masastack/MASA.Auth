namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class RolePermission : AuditEntity<Guid, Guid>, ISoftDelete
{
    public Role Role { get; set; } = null!;

    public bool IsDeleted { get; private set; }

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public RolePermission(Guid permissionId)
    {
        PermissionId = permissionId;
    }
}

