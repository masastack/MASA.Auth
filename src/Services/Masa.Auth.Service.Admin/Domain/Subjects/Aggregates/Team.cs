// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Subjects.Aggregates;

public class Team : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public AvatarValue Avatar { get; private set; }

    public string Description { get; private set; }

    public TeamTypes TeamType { get; private set; }

    private List<TeamStaff> _teamStaffs = new();

    public IReadOnlyCollection<TeamStaff> TeamStaffs => _teamStaffs;

    private List<TeamPermission> _teamPermissions = new();

    public IReadOnlyCollection<TeamPermission> TeamPermissions => _teamPermissions;

    private List<TeamRole> _teamRoles = new();

    public IReadOnlyCollection<TeamRole> TeamRoles => _teamRoles;

    public Team(string name, string description, TeamTypes teamType, AvatarValue avatar) : this(Guid.Empty, name, description, teamType, avatar)
    {
    }

    public Team(Guid id, string name, string description, TeamTypes teamType, AvatarValue avatar)
    {
        Id = id;
        Name = name;
        Description = description;
        TeamType = teamType;
        Avatar = avatar;
    }

    public Team(string name, string description, TeamTypes teamType)
        : this(name, description, teamType, new AvatarValue(name, ""))
    {
    }

    public void SetId(Guid id)
    {
        Id = id;
    }

    public void UpdateBasicInfo(string name, string description, TeamTypes teamType, AvatarValue avatar)
    {
        Name = name;
        Description = description;
        TeamType = teamType;
        Avatar = avatar;
    }

    public void SetStaff(TeamMemberTypes memberType, IEnumerable<Staff> staffs)
    {
        var teamStaffs = _teamStaffs.Where(ts => ts.TeamMemberType == memberType).ToList();
        teamStaffs = teamStaffs.MergeBy(
           staffs.Select(staff => new TeamStaff(staff.Id, memberType, staff.UserId)),
           item => item.StaffId);
        teamStaffs.AddRange(_teamStaffs.Where(ts => ts.TeamMemberType != memberType));
        _teamStaffs = teamStaffs;
    }

    public void SetPermission(TeamMemberTypes teamMemberType, List<SubjectPermissionRelationDto> permissions)
    {
        var teamPermissions = _teamPermissions.Where(ts => ts.TeamMemberType == teamMemberType).ToList();
        teamPermissions = teamPermissions.MergeBy(
           permissions.Select(permission => new TeamPermission(permission.PermissionId, permission.Effect, teamMemberType)),
           item => item.PermissionId,
           (oldValue, newValue) =>
           {
               oldValue.Update(newValue.Effect);
               return oldValue;
           });
        teamPermissions.AddRange(_teamPermissions.Where(ts => ts.TeamMemberType != teamMemberType));
        _teamPermissions = teamPermissions;
    }

    public void SetRole(TeamMemberTypes memberType, params Guid[] roleIds)
    {
        var teamRoles = _teamRoles.Where(ts => ts.TeamMemberType == memberType).ToList();
        teamRoles = teamRoles.MergeBy(
          roleIds.Select(roleId => new TeamRole(roleId, memberType)),
           item => item.RoleId);
        teamRoles.AddRange(_teamRoles.Where(ts => ts.TeamMemberType != memberType));
        _teamRoles = teamRoles;
    }

    public List<Guid> GetAdminRoleIds()
    {
        return _teamRoles.Where(ts => ts.TeamMemberType == TeamMemberTypes.Admin).Select(a => a.RoleId).ToList();
    }

    public List<Guid> GetMemberRoleIds()
    {
        return _teamRoles.Where(ts => ts.TeamMemberType == TeamMemberTypes.Member).Select(a => a.RoleId).ToList();
    }
}

