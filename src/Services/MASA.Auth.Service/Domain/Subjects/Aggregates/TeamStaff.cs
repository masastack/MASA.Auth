namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class TeamStaff : Entity<Guid>
{
    public Guid TeamId { get; private set; }

    public Guid StaffId { get; private set; }

    public Guid UserId { get; private set; }

    public TeamStaff(Guid teamId, Guid staffId, Guid userId)
    {
        TeamId = teamId;
        StaffId = staffId;
        UserId = userId;
    }
}

public enum TeamStaffType
{
    Member,
    Admin,
}

