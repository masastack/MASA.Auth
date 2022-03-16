namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class UpdateTeamRequest
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public AvatarValue Avatar { get; set; }

    public string Description { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> Staffs { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> Roles { get; set; }

    public UpdateTeamRequest(Guid teamId, string name, AvatarValue avatar, string description, TeamTypes teamType, List<Guid> staffs, List<Guid> permissions, List<Guid> roles)
    {
        TeamId = teamId;
        Name = name;
        Avatar = avatar;
        Description = description;
        TeamType = teamType;
        Staffs = staffs;
        Permissions = permissions;
        Roles = roles;
    }

    public static implicit operator UpdateTeamRequest(TeamDetailResponse team)
    {
        return new UpdateTeamRequest(team.TeamId, team.Name, team.Avatar, team.Describe, team.TeamType, team.Staffs, team.Permissions, team.Roles);
    }
}


