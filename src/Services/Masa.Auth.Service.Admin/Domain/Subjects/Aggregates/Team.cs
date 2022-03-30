namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Team : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public AvatarValue Avatar { get; private set; }

    public string Description { get; private set; }

    public TeamTypes TeamType { get; private set; }

    public List<TeamStaff> Staffs { get; private set; } = new();

    public List<TeamPermission> Permissions { get; private set; } = new();

    public List<TeamRole> Roles { get; private set; } = new();

    public Team(string name, string description, TeamTypes teamType, AvatarValue avatar)
    {
        Name = name;
        Description = description;
        TeamType = teamType;
        Avatar = avatar;
    }

    public Team(string name, string description, TeamTypes teamType)
        : this(name, description, teamType, new AvatarValue(name, ""))
    {
    }

    public void InitRole()
    {

    }
}

