namespace Masa.Auth.Service.Domain.Subjects.Aggregates;

public class Team : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string? Avatar { get; private set; }

    public string? Describe { get; private set; }

    public List<TeamStaff> Staffs { get; private set; } = new();

    public List<TeamPermission> Permissions { get; private set; } = new();

    public List<TeamRole> Roles { get; private set; } = new();

    public Team(string name, string? avatar, string? describe)
    {
        Name = name;
        Avatar = avatar;
        Describe = describe;
    }
}

