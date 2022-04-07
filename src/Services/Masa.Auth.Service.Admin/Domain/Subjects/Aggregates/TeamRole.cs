namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class TeamRole : AuditEntity<Guid, Guid>
{
    public Team Team { get; private set; } = null!;

    public Guid RoleId { get; private set; }

    public TeamMemberTypes TeamMemberType { get; private set; }

    public TeamRole(Guid roleId, TeamMemberTypes teamMemberType)
    {
        RoleId = roleId;
        TeamMemberType = teamMemberType;
    }
}

