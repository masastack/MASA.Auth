namespace Masa.Auth.ApiGateways.Caller.Response.Subjects;

public class TeamDetailResponse
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public AvatarValue Avatar { get; set; }

    public string Describe { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> Staffs { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> Roles { get; set; }

    public static TeamDetailResponse Default => new(Guid.Empty, "", new(), "", default, new(), new(), new());

    public TeamDetailResponse(Guid teamId, string name, AvatarValue avatar, string describe, TeamTypes teamType, List<Guid> staffs, List<Guid> permissions, List<Guid> roles)
    {
        TeamId = teamId;
        Name = name;
        Avatar = avatar;
        Describe = describe;
        TeamType = teamType;
        Staffs = staffs;
        Permissions = permissions;
        Roles = roles;
    }
}


