namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class PermissionRelation : AuditEntity<Guid, Guid>, ISoftDelete
{
    public Guid ChildPermissionId { get; private set; }

    public Guid PermissionId { get; private set; }

    public bool IsDeleted { get; private set; }

    public Permission ChildPermission { get; private set; } = null!;

    public PermissionRelation(Guid childPermissionId)
    {
        ChildPermissionId = childPermissionId;
    }
}

