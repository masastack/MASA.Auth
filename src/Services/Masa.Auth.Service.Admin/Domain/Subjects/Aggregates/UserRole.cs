namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserRole : AuditEntity<Guid, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public User User { get; set; } = null!;

    public Guid RoleId { get; set; }

    public UserRole(Guid roleId)
    {
        RoleId = roleId;
    }
}
