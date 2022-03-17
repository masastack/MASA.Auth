namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Team : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public AvatarValue Avatar { get; private set; }

    public string Describe { get; private set; }

    public TeamTypes TeamType { get; private set; }

    public AvatarValue Avatar { get; private set; }

    public List<TeamStaff> Staffs { get; private set; } = new();

    public List<TeamPermission> Permissions { get; private set; } = new();

    public List<TeamRole> Roles { get; private set; } = new();

    public Team(string name, string describe, TeamTypes teamType, AvatarValue avatar)
    {
        Name = name;
        Describe = describe;
        TeamType = teamType;
        Avatar = avatar;
    }

    public Team(string name, string describe, TeamTypes teamType)
        : this(name, describe, teamType, new AvatarValue(name, ""))
    {
    }
}

