namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class TeamPermission : Entity<Guid>
{
    private Team? _team;
    public Team Team
    {
        get => _team ?? throw new UserFriendlyException("Failed to get team data");
        private set => _team = value;
    }

    public Guid TeamId { get; private set; }

    public Guid PermissionId { get; private set; }

    public Permission Permission { get; private set; } = null!;

    public bool Effect { get; private set; }

    public TeamStaffType TeamStaffType { get; private set; }

    public TeamPermission(Guid teamId, Guid permissionId, bool effect, TeamStaffType teamStaffType)
    {
        TeamId = teamId;
        PermissionId = permissionId;
        Effect = effect;
        TeamStaffType = teamStaffType;
    }
}

