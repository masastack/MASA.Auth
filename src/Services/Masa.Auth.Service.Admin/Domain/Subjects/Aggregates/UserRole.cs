namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserRole : AuditEntity<Guid, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public User User { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public Role Role { get; set; } = null!;

    public UserRole(Guid roleId)
    {
        RoleId = roleId;
    }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
    }
}
