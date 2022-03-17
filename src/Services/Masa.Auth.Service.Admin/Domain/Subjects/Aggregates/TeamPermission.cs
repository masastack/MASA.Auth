namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamPermission : Entity<Guid>
{
    private Team? _team;
    private Permission? _permission;

    public Team Team
    {
        get => _team ?? throw new UserFriendlyException("Failed to get team data");
        private set => _team = value;
    }

    public Permission Permission
    {
        get => _permission ?? throw new UserFriendlyException("Failed to get permission data");
        private set => _permission = value;
    }

    public Guid TeamId { get; private set; }

    public Guid PermissionId { get; private set; }

    public bool Effect { get; private set; }

    public TeamMemberTypes TeamStaffType { get; private set; }

    public TeamPermission(Guid teamId, Guid permissionId, bool effect, TeamMemberTypes teamStaffType)
    {
        TeamId = teamId;
        PermissionId = permissionId;
        Effect = effect;
        TeamStaffType = teamStaffType;
    }
}

