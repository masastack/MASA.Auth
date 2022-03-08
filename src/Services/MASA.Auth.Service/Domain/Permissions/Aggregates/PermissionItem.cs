namespace Masa.Auth.Service.Domain.Permissions.Aggregates;

public class PermissionItem : Entity<Guid>
{
    public Guid ParentId { get; private set; }

    public Permission Permission { get; private set; } = null!;
}

