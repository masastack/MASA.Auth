namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class TeamRole : Entity<Guid>
{
    public Guid TeamId { get; private set; }

    public Guid RoleId { get; private set; }

    public TeamRole(Guid teamId, Guid roleId)
    {
        TeamId = teamId;
        RoleId = roleId;
    }
}

