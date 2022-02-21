namespace MASA.Auth.Service.Domain.Permissions.Aggregates.Roles;

public class RolePermission : Entity<Guid>
{
    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;

    public bool Effect { get; private set; }
}

