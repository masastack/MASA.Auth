namespace Masa.Auth.Service.Admin.Domain.Permissions.Aggregates;

public class RoleRelation : AuditEntity<Guid, Guid>
{
    public Guid ParentId { get; private set; }

    public Role Role { get; set; } = null!;
}

