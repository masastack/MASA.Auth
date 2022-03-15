﻿using Masa.Auth.ApiGateways.Caller.Enums;
using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.ApiGateways.Caller.Request.Subjects;

public class AddTeamRequest
{
    public string Name { get; set; }

    public string Avatar { get; set; }

    public string AvatarName { get; set; }

    public string Describe { get; set; }

    public TeamTypes TeamType { get; set; }

    public List<Guid> Staffs { get; set; }

    public List<Guid> Permissions { get; set; }

    public List<Guid> Roles { get; set; }

    public AddTeamRequest(string name, string avatar, string avatarName, string describe, TeamTypes teamType, List<Guid> staffs, List<Guid> permissions, List<Guid> roles)
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

    public static implicit operator AddTeamRequest(TeamDetailResponse team)
    {
        return new AddTeamRequest(team.Name, team.Avatar, team.AvatarName, team.Describe, team.TeamType, team.Staffs, team.Permissions, team.Roles);
    }
}


