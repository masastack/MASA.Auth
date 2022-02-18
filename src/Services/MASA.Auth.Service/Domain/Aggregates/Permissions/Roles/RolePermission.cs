namespace MASA.Auth.Service.Domain.Aggregate.Permissions;

public class RolePermission : Entity<Guid>
{
    public Permission Permission { get; set; } = null!;

    public bool Effect { get; private set; }

    public Role Role { get; set; } = null!;
}

