namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class TeamRole : Entity<Guid>
{
    private Team? _team;
    public Team Team
    {
        get => _team ?? throw new UserFriendlyException("Failed to get team data");
        private set => _team = value;
    }

    public Guid TeamId { get; private set; }

    public Guid RoleId { get; private set; }

    public TeamMemberTypes TeamStaffType { get; private set; }

    public TeamRole(Guid teamId,Guid roleId, TeamMemberTypes teamStaffType)
    {
        TeamId = teamId;
        RoleId = roleId;
        TeamStaffType = teamStaffType;
    }
}

