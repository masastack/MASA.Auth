namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class PermissionRelation : Entity<Guid>
{
    public Guid ChildId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    public PermissionRelation(Guid childId)
    {
        ChildId = childId;
    }
}

