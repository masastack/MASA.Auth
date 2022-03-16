using Masa.Auth.Service.Admin.Infrastructure.Enums;

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Team : AuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public string Avatar { get; private set; }

    public string AvatarName { get; set; }

    public string Describe { get; private set; }

    public TeamTypes TeamType { get; private set; }

    public List<TeamStaff> Staffs { get; private set; } = new();

    public List<TeamPermission> Permissions { get; private set; } = new();

    public List<TeamRole> Roles { get; private set; } = new();

    public Team(string name, string avatar, string avatarName, string describe, TeamTypes teamType)
    {
        Name = name;
        Avatar = avatar;
        AvatarName = avatarName;
        Describe = describe;
        TeamType = teamType;
    }
}

