namespace Masa.Auth.Service.Domain.Permissions.Aggregates;

public class RolePermission : Entity<Guid>
{
    public Role Role { get; set; } = null!;

    public Permission Permission { get; set; } = null!;

    public bool Effect { get; private set; }

    public bool Private { get; private set; }
}

