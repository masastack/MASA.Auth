using Masa.Contrib.BasicAbilities.Auth.Enums;

namespace Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

public class TeamDetailResponse
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string AvatarName { get; set; }

    public string Describe { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> Staffs { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> Roles { get; set; }

    public static TeamDetailResponse Default => new("", "", "", "", default, new(), new(), new());

    public TeamDetailResponse(string name, string avatar, string avatarName, string describe, TeamTypes teamType, List<Guid> staffs, List<Guid> permissions, List<Guid> roles)
    {
        Name = name;
        Avatar = avatar;
        AvatarName = avatarName;
        Describe = describe;
        TeamType = teamType;
        Staffs = staffs;
        Permissions = permissions;
        Roles = roles;
    }
}


