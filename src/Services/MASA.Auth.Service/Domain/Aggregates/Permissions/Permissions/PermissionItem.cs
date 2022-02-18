namespace MASA.Auth.Service.Domain.Aggregate.Permissions;

public class PermissionItem : Entity<Guid>
{
    public Guid ParentId { get; private set; }

    public Permission Permission { get; private set; } = null!;
}

