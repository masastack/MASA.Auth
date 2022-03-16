namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class TeamStaff : Entity<Guid>
{
    private Team? _team;
    public Team Team
    {
        get => _team ?? throw new UserFriendlyException("Failed to get team data");
        private set => _team = value;
    }

    public Guid TeamId { get; private set; }

    public Guid StaffId { get; private set; }

    public Guid UserId { get; private set; }

    public TeamMemberTypes TeamStaffType { get; private set; }

    public TeamStaff(Guid teamId, Guid staffId, Guid userId, TeamMemberTypes teamStaffType)
    {
        TeamId = teamId;
        StaffId = staffId;
        UserId = userId;
        TeamStaffType = teamStaffType;
    }
}


