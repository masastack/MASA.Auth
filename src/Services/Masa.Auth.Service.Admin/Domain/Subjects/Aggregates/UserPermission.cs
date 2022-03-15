namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class UserPermission : Entity<Guid>
{
    private User? _user;
    private Permission? _permission;

    public User User
    {
        get => _user ?? throw new UserFriendlyException("Failed to get user data");
        set => _user = value;
    }

    public Permission Permission
    {
        get => _permission ?? throw new UserFriendlyException("Failed to get permission data");
        set => _permission = value;
    }

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
