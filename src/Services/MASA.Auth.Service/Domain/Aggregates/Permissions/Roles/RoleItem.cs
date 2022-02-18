namespace MASA.Auth.Service.Domain.Aggregate.Permissions;

public class RoleItem : Entity<Guid>
{
    public Guid ParentId { get; private set; }

    public Role Role { get; set; } = null!;
}

