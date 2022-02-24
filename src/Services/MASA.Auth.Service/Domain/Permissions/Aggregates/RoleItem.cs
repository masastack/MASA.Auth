namespace MASA.Auth.Service.Domain.Permissions.Aggregates;

public class RoleItem : Entity<Guid>
{
    public Guid ParentId { get; private set; }

    public Role Role { get; set; } = null!;
}

