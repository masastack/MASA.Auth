using Masa.Contrib.BasicAbilities.Auth.Enums;
using Masa.Contrib.BasicAbilities.Auth.Response.Subjects;

namespace Masa.Contrib.BasicAbilities.Auth.Request.Subjects;

public class EditTeamRequest
{
    public Guid TeamId { get; set; }

    public string Name { get; set; }

    public string Avatar { get; set; }

    public string AvatarName { get; set; }

    public string Description { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> Staffs { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> Roles { get; set; }

    public EditTeamRequest(Guid teamId, string name, string avatar, string avatarName, string description, TeamTypes teamType, List<Guid> staffs, List<Guid> permissions, List<Guid> roles)
    {
        TeamId = teamId;
        Name = name;
        Avatar = avatar;
        AvatarName = avatarName;
        Description = description;
        TeamType = teamType;
        Staffs = staffs;
        Permissions = permissions;
        Roles = roles;
    }

    public static implicit operator EditTeamRequest(TeamDetailResponse team)
    {
        return new EditTeamRequest(team.TeamId, team.Name, team.Avatar, team.AvatarName, team.Describe, team.TeamType, team.Staffs, team.Permissions, team.Roles);
    }
}


