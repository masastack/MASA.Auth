namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class UserRole : AuditEntity<Guid, Guid>
{
    public Guid UserId { get; set; }

    public Guid RoleId { get; set; }

    public UserRole(Guid userId, Guid roleId)
    {
        UserId = userId;
        RoleId = roleId;
    }
}
