namespace MASA.Auth.RolePermission.Domain.Aggregate;

public class RoleItem : Entity<int>
{
    public int ChildrenRoleId { get; set; }

    public Role Role { get; set; } = null!;
}

