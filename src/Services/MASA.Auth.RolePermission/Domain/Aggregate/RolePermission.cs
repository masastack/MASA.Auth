namespace MASA.Auth.RolePermission.Domain.Aggregate;

public class RolePermission : Entity<int>
{
    public int PermissionId { get; set; }

    public bool Effect { get; set; }

    public Role Role { get; set; } = null!;
}

