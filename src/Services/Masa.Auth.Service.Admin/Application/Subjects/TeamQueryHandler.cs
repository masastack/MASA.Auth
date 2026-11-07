// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class TeamQueryHandler
{
    private readonly AuthDbContext _authDbContext;
    private readonly ITeamRepository _teamRepository;
    private readonly IStaffRepository _staffRepository;
    private readonly IDistributedCacheClient _distributedCacheClient;

    public TeamQueryHandler(
        AuthDbContext authDbContext,
        ITeamRepository teamRepository,
        IStaffRepository staffRepository,
        IDistributedCacheClient distributedCacheClient)
    {
        _authDbContext = authDbContext;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _distributedCacheClient = distributedCacheClient;
    }

    [EventHandler]
    public async Task TeamListAsync(TeamListQuery teamListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (!string.IsNullOrWhiteSpace(teamListQuery.Name))
        {
            condition = condition.And(t => t.Name.Contains(teamListQuery.Name));
        }
        var staffId = Guid.Empty;
        if (teamListQuery.UserId != Guid.Empty)
        {
            var user = await _authDbContext.Set<User>().FirstOrDefaultAsync(u => u.Id == teamListQuery.UserId);
            if (user == null)
            {
                return;
            }
            if (!user.IsAdmin())
            {
                staffId = (await _authDbContext.Set<Staff>()
                                        .FirstOrDefaultAsync(staff => staff.UserId == teamListQuery.UserId))?.Id ?? Guid.Empty;
                if (staffId == default)
                {
                    return;
                }
                condition = condition.And(t => t.TeamStaffs.Any(s => s.StaffId == staffId));
            }
        }
        var teams = await _authDbContext.Set<Team>()
                                        .Include(t => t.TeamStaffs)
                                        .Where(condition)
                                        .AsNoTracking()
                                        .AsSplitQuery()
                                        .OrderByDescending(t => t.ModificationTime)
                                        .ToListAsync();

        var allAdminStaffIds = teams
            .SelectMany(team => team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin)
            .Select(s => s.StaffId))
            .Distinct()
            .ToList();

        var adminStaffs = allAdminStaffIds.Any()
            ? await _staffRepository.GetListAsync(s => allAdminStaffIds.Contains(s.Id))
            : new List<Staff>();
        var adminStaffAvatarDict = adminStaffs.ToDictionary(s => s.Id, s => s.Avatar);

        var modifierIds = teams.Select(t => t.Modifier).Distinct().ToList();
        var userModels = await _distributedCacheClient.GetListAsync<UserModel>(modifierIds.Select(CacheKey.UserKey).ToList());
        var userModelDict = userModels.Where(u => u != null).ToDictionary(u => u!.Id, u => u);

        foreach (var team in teams)
        {
            var modifierName = userModelDict.TryGetValue(team.Modifier, out var userModel) ? userModel!.RealDisplayName : "";
            var staffIds = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin).Select(s => s.StaffId);
            var adminAvatar = staffIds.Select(id => adminStaffAvatarDict.TryGetValue(id, out var avatar) ? avatar : "").ToList();

            var teamDto = new TeamDto(team.Id, team.Name, $"{team.Avatar.Url}?stamp={team.ModificationTime.Second}", team.Description, team.TeamStaffs.Count,
                adminAvatar, modifierName, team.ModificationTime);
            if (staffId != Guid.Empty)
            {
                teamDto.Role = team.TeamStaffs.FirstOrDefault(ts => ts.StaffId == staffId)?.TeamMemberType.ToString() ?? "";
            }
            teamListQuery.Result.Add(teamDto);
        }
    }

    [EventHandler]
    public async Task TeamDetailAsync(TeamDetailQuery teamDetailQuery)
    {
        var team = await _teamRepository.GetByIdAsync(teamDetailQuery.TeamId);
        teamDetailQuery.Result = new TeamDetailDto
        {
            Id = team.Id,
            TeamBasicInfo = new TeamBasicInfoDto
            {
                Name = team.Name,
                Description = team.Description,
                Type = (int)team.TeamType,
                Avatar = new AvatarValueDto
                {
                    Url = team.Avatar.Url,
                    Name = team.Avatar.Name,
                    Color = team.Avatar.Color
                }
            },
            TeamAdmin = new TeamPersonnelDto
            {
                Staffs = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin).Select(s => s.StaffId).ToList(),
                Roles = team.TeamRoles.Where(r => r.TeamMemberType == TeamMemberTypes.Admin).Select(r => r.RoleId).ToList(),
                Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Admin).Select(tp => (SubjectPermissionRelationDto)tp).ToList()
            },
            TeamMember = new TeamPersonnelDto
            {
                Staffs = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Member).Select(s => s.StaffId).ToList(),
                Roles = team.TeamRoles.Where(r => r.TeamMemberType == TeamMemberTypes.Member).Select(r => r.RoleId).ToList(),
                Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Member).Select(tp => (SubjectPermissionRelationDto)tp).ToList()
            }
        };
    }

    [EventHandler]
    public async Task TeamDetailForExternalAsync(TeamDetailForExternalQuery query)
    {
        var team = await _authDbContext.Set<Team>()
                                       .Include(t => t.TeamStaffs)
                                       .ThenInclude(ts => ts.Staff)
                                       .Include(t => t.TeamRoles)
                                       .ThenInclude(tr => tr.Role)
                                       .AsNoTracking()
                                       .AsSplitQuery()
                                       .FirstOrDefaultAsync(t => t.Id == query.TeamId);
        if (team != null)
        {
            query.Result = new TeamDetailModel
            {
                Id = team.Id,
                Name = team.Name,
                Description = team.Description,
                TeamType = team.TeamType,
                Avatar = team.Avatar.Url,
                Admins = team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Admin).Select(ts => ts.Staff.Adapt<StaffModel>()).ToList(),
                Members = team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Member).Select(ts => ts.Staff.Adapt<StaffModel>()).ToList(),
                AdminRoles = team.TeamRoles.Where(tr => tr.TeamMemberType == TeamMemberTypes.Admin).Select(tr => tr.Role.Adapt<RoleModel>()).ToList(),
                MemberRoles = team.TeamRoles.Where(tr => tr.TeamMemberType == TeamMemberTypes.Member).Select(tr => tr.Role.Adapt<RoleModel>()).ToList()
            };
        }
    }

    [EventHandler]
    public async Task TeamSelectListAsync(TeamSelectListQuery teamSelectListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (!string.IsNullOrEmpty(teamSelectListQuery.Name))
        {
            condition = condition.And(s => s.Name.Contains(teamSelectListQuery.Name));
        }
        teamSelectListQuery.Result = (await _teamRepository.GetListAsync(condition))
                .Select(t => new TeamSelectDto(t.Id, t.Name, t.Avatar.Url))
                .ToList();
    }

    [EventHandler]
    public async Task GetTeamRoleSelectAsync(TeamRoleSelectQuery query)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (!string.IsNullOrEmpty(query.Name))
        {
            condition = condition.And(team => team.Name.Contains(query.Name));
        }
        if (query.UserId != default)
        {
            condition = condition.And(team => team.TeamStaffs.Any(ts => ts.UserId == query.UserId));
        }
        var teams = await _authDbContext.Set<Team>()
                                        .Where(condition)
                                        .Include(team => team.TeamStaffs)
                                        .Include(team => team.TeamRoles)
                                        .ThenInclude(tr => tr.Role)
                                        .AsNoTracking()
                                        .ToListAsync();
        foreach (var team in teams)
        {
            var teamRoles = team.TeamRoles.ToList();
            if (query.UserId != default)
            {
                var memberType = team.TeamStaffs.FirstOrDefault(ts => ts.UserId == query.UserId)?.TeamMemberType;
                if (memberType != null)
                {
                    teamRoles = teamRoles.Where(tr => tr.TeamMemberType == memberType).ToList();
                }
            }
            var roles = teamRoles.DistinctBy(r => r.RoleId)
                            .Select(tr => new RoleSelectDto(tr.Role.Id, tr.Role.Name, tr.Role.Code,
                            tr.Role.Limit, tr.Role.AvailableQuantity))
                            .ToList();
            query.Result.Add(new TeamRoleSelectDto(team.Id, team.Name, team.Avatar.Url, roles));
        }
    }

    [EventHandler]
    public async Task GetTeamByUserAsync(TeamByUserQuery query)
    {
        query.Result = await _authDbContext.Set<TeamStaff>()
                                        .Where(ts => ts.UserId == query.UserId)
                                        .Select(team => new TeamSampleDto(team.TeamId, team.TeamMemberType))
                                        .ToListAsync();
    }
}
