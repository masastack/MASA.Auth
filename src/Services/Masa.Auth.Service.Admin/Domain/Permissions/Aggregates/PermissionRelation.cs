namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class PermissionRelation : AuditEntity<Guid, Guid>
{
    public Guid ChildId { get; private set; }

    public Permission ChildPermission { get; private set; } = null!;

    public Permission Permission { get; private set; } = null!;

    public PermissionRelation(Guid childId)
    {
        ChildId = childId;
    }
}

