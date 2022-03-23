namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamStaff : AuditEntity<Guid, Guid>
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

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamStaff(Guid teamId, Guid staffId, Guid userId, TeamMemberTypes teamMemberType)
    {
        TeamId = teamId;
        StaffId = staffId;
        UserId = userId;
        TeamMemberType = teamMemberType;
    }
}


