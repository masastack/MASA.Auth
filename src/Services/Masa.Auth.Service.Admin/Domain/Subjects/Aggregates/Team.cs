// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Team : FullAuditAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public AvatarValue Avatar { get; private set; }

    public string Description { get; private set; }

    public TeamTypes TeamType { get; private set; }

    public int MemberCount { get; private set; }

    private List<TeamStaff> teamStaffs = new();

    public IReadOnlyCollection<TeamStaff> TeamStaffs => teamStaffs;

    private List<TeamPermission> teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => teamPermissions;

    private List<TeamRole> teamRoles = new();

    public IReadOnlyCollection<TeamRole> TeamRoles => teamRoles;

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

    public void UpdateBasicInfo(string name, string description, TeamTypes teamType, AvatarValue avatar)
    {
        Name = name;
        Description = description;
        TeamType = teamType;
        Avatar = avatar;
    }

    public void SetStaff(TeamMemberTypes memberType, List<Guid> staffIds)
    {
        teamStaffs.RemoveAll(ts => ts.TeamMemberType == memberType);
        teamStaffs.AddRange(staffIds.Select(s => new TeamStaff(s, memberType)));
        MemberCount = teamStaffs.Count;
    }

    public void SetPermission(TeamMemberTypes memberType, Dictionary<Guid, bool> permissionsIds)
    {
        teamPermissions.RemoveAll(ts => ts.TeamMemberType == memberType);
        teamPermissions.AddRange(permissionsIds.Select(p => new TeamPermission(p.Key, p.Value, memberType)));
    }

    public Guid GetAdminRoleId()
    {
        return teamRoles.FirstOrDefault(ts => ts.TeamMemberType == TeamMemberTypes.Admin)?.RoleId ?? Guid.Empty;
    }

    public Guid GetMemberRoleId()
    {
        return teamRoles.FirstOrDefault(ts => ts.TeamMemberType == TeamMemberTypes.Member)?.RoleId ?? Guid.Empty;
    }

    public void SetRole(TeamMemberTypes memberType, params Guid[] roleIds)
    {
        teamRoles.RemoveAll(tr => tr.TeamMemberType == memberType);
        teamRoles.AddRange(roleIds.Select(roleId => new TeamRole(roleId, memberType)));
    }
}

