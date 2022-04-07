namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamStaff : AuditEntity<Guid, Guid>
{
    public Guid TeamId { get; private set; }

    public Guid StaffId { get; private set; }

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamStaff(Guid staffId, TeamMemberTypes teamMemberType)
    {
        StaffId = staffId;
        TeamMemberType = teamMemberType;
    }
    public TeamStaff(Guid teamId, Guid staffId, TeamMemberTypes teamMemberType)
    {
        TeamId = teamId;
        TeamMemberType = teamMemberType;
    }
}


