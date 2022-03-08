namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class UserPermission : Entity<Guid>
{
    public Guid UserId { get; private set; }

    public Guid PermissionId { get; private set; }

    public bool Effect { get; private set; }

    public UserPermission(Guid userId, Guid permissionId, bool effect)
    {
        UserId = userId;
        PermissionId = permissionId;
        Effect = effect;
    }
}
