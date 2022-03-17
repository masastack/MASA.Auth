namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class RolePermission : Entity<Guid>
{
    public Role Role { get; set; } = null!;

    public Guid PermissionId { get; set; }

    public Permission Permission { get; set; } = null!;

    public RolePermission(Guid permissionId)
    {
        PermissionId = permissionId;
    }
}

