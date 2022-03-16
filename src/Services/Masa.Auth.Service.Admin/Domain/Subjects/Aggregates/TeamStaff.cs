using Masa.Auth.Service.Admin.Infrastructure.Enums;

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

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

    public TeamStaffType TeamStaffType { get; private set; }

    public TeamStaff(Guid teamId, Guid staffId, Guid userId, TeamStaffType teamStaffType)
    {
        TeamId = teamId;
        StaffId = staffId;
        UserId = userId;
        TeamStaffType = teamStaffType;
    }
}


