namespace MASA.Auth.Service.Domain.Subjects.Aggregates;

public class TeamPermission : Entity<Guid>
{
    public Guid TeamId { get; private set; }

    public Guid PermissionId { get; private set; }

    public bool Effect { get; private set; }

    public TeamPermission(Guid teamId, Guid permissionId, bool effect)
    {
        TeamId = teamId;
        PermissionId = permissionId;
        Effect = effect;
    }
}

