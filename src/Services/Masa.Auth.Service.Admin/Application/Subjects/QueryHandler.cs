// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Subjects;

public class QueryHandler
{
    readonly IUserRepository _userRepository;
    readonly ITeamRepository _teamRepository;
    readonly IStaffRepository _staffRepository;
    readonly IThirdPartyUserRepository _thirdPartyUserRepository;
    readonly IThirdPartyIdpRepository _thirdPartyIdpRepository;
    readonly ILdapIdpRepository _ldapIdpRepository;
    readonly AuthDbContext _authDbContext;
    readonly IAutoCompleteClient _autoCompleteClient;
    readonly IMemoryCacheClient _memoryCacheClient;
    readonly IUserSystemBusinessDataRepository _userSystemBusinessDataRepository;

    public QueryHandler(
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IStaffRepository staffRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        ILdapIdpRepository ldapIdpRepository,
        AuthDbContext authDbContext,
        IAutoCompleteClient autoCompleteClient,
        IMemoryCacheClient memoryCacheClient,
        IUserSystemBusinessDataRepository userSystemBusinessDataRepository)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _ldapIdpRepository = ldapIdpRepository;
        _authDbContext = authDbContext;
        _autoCompleteClient = autoCompleteClient;
        _memoryCacheClient = memoryCacheClient;
        _userSystemBusinessDataRepository = userSystemBusinessDataRepository;
    }

    #region User

    [EventHandler]
    public async Task GetUsersAsync(UsersQuery query)
    {
        Expression<Func<User, bool>> condition = user => true;
        if (query.Enabled is not null)
            condition = condition.And(user => user.Enabled == query.Enabled);

        if (query.StartTime is not null)
            condition = condition.And(user => user.CreationTime >= query.StartTime);

        if (query.EndTime is not null)
            condition = condition.And(user => user.CreationTime <= query.EndTime);

        if (query.UserId != Guid.Empty)
            condition = condition.And(user => user.Id == query.UserId);

        var users = await _userRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        query.Result = new(users.Total, users.Result.Select(u => (UserDto)u).ToList());
    }

    [EventHandler]
    public async Task GetUserDetailAsync(UserDetailQuery query)
    {
        var user = await _userRepository.GetDetailAsync(query.UserId);
        if (user is null) throw new UserFriendlyException("This user data does not exist");
        var creator = await _authDbContext.Set<User>().Where(u => u.Id == user.Creator).Select(u => u.Name).FirstOrDefaultAsync();
        var modifier = await _authDbContext.Set<User>().Where(u => u.Id == user.Modifier).Select(u => u.Name).FirstOrDefaultAsync();

        query.Result = user;
        query.Result.Creator = creator ?? "";
        query.Result.Modifier = modifier ?? "";
    }

    [EventHandler]
    public async Task FindUserByAccountQueryAsync(FindUserByAccountQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Account == query.Account);
        if (user is null)
        {
            throw new UserFriendlyException("This user data does not exist");
        }
        query.Result = user;
    }

    [EventHandler]
    public async Task FindUserByEmailQueryAsync(FindUserByEmailQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.Email == query.Email);
        if (user is null)
        {
            throw new UserFriendlyException("This user data does not exist");
        }
        query.Result = user;
    }

    [EventHandler]
    public async Task FindUserByPhoneNumberQueryAsync(FindUserByPhoneNumberQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.PhoneNumber == query.PhoneNumber);
        if (user is null)
        {
            throw new UserFriendlyException("This user data does not exist");
        }
        query.Result = user;
    }


    [EventHandler]
    public async Task GetUserSelectAsync(UserSelectQuery query)
    {
        var response = await _autoCompleteClient.GetAsync<UserSelectDto, Guid>(
            query.Search.TrimStart(' ').TrimEnd(' ')
        );
        query.Result = response.Data;
    }

    [EventHandler]
    public async Task GetAllUsers(AllUsersQuery query)
    {
        var users = await _userRepository.GetAllAsync();
        query.Result = users;
    }

    [EventHandler]
    public async Task UserPortraitsQueryAsync(UserPortraitsQuery userPortraitsQuery)
    {
        foreach (var userId in userPortraitsQuery.UserIds)
        {
            var userCache = await _memoryCacheClient.GetAsync<CacheUser>(CacheKey.UserKey(userId));
            if (userCache != null)
            {
                userPortraitsQuery.Result.Add(new UserPortraitModel
                {
                    Id = userId,
                    Name = userCache.Name,
                    DisplayName = userCache.DisplayName,
                    Avatar = userCache.Avatar,
                    Account = userCache.Account
                });
            }
        }
    }

    #endregion

    #region Staff

    [EventHandler]
    public async Task GetStaffsAsync(StaffsQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (query.Enabled is not null)
            condition = condition.And(s => s.Enabled == query.Enabled);

        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));

        if (query.DepartmentId != Guid.Empty)
        {
            var staffIds = _authDbContext.Set<DepartmentStaff>()
                .Where(ds => ds.DepartmentId == query.DepartmentId)
                .Select(ds => ds.StaffId);
            condition = condition.And(s => staffIds.Contains(s.Id));
        }
        var staffQuery = _authDbContext.Set<Staff>().Where(condition);
        var total = await staffQuery.LongCountAsync();
        var staffs = await staffQuery
                                    .Include(s => s.DepartmentStaffs)
                                    .ThenInclude(ds => ds.Department)
                                    .Include(s => s.Position)
                                    .OrderByDescending(s => s.ModificationTime)
                                    .ThenByDescending(s => s.CreationTime)
                                    .Skip((query.Page - 1) * query.PageSize)
                                    .Take(query.PageSize)
                                    .ToListAsync();

        query.Result = new(total, staffs.Select(staff => (StaffDto)staff).ToList());
    }

    [EventHandler]
    public async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.User)
                                        .Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .Include(s => s.CreateUser)
                                        .Include(s => s.ModifyUser)
                                        .FirstOrDefaultAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException("This staff data does not exist");

        query.Result = staff;
    }

    [EventHandler]
    public async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));
        var staffs = await _staffRepository.GetPaginatedListAsync(condition, 0, query.MaxCount);

        query.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.Avatar)).ToList();
    }

    [EventHandler]
    public async Task GetStaffSelectByIdsAsync(StaffSelectByIdQuery staffSelectByIdQuery)
    {
        var staffs = await _staffRepository.GetListAsync(s => staffSelectByIdQuery.Ids.Contains(s.Id));
        staffSelectByIdQuery.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.Avatar)).ToList();
    }

    [EventHandler]
    public async Task GetStaffsByDepartmentAsync(StaffsByDepartmentQuery query)
    {
        var staffs = await _authDbContext.Set<Staff>()
                                         .Include(s => s.Position)
                                         .Include(staff => staff.DepartmentStaffs)
                                         .ThenInclude(departmentStaff => departmentStaff.Department)
                                         .Where(staff => staff.DepartmentStaffs.Any(department => department.DepartmentId == query.DepartmentId))
                                         .ToListAsync();

        query.Result = staffs.Select(staff => (StaffDto)staff).ToList();
    }

    [EventHandler]
    public async Task GetStaffsByTeamAsync(StaffsByTeamQuery query)
    {
        var staffs = await _authDbContext.Set<Staff>()
                                         .Include(s => s.Position)
                                         .Include(staff => staff.DepartmentStaffs)
                                         .ThenInclude(departmentStaff => departmentStaff.Department)
                                         .Include(staff => staff.TeamStaffs)
                                         .Where(staff => staff.TeamStaffs.Any(team => team.TeamId == query.TeamId))
                                         .ToListAsync();

        query.Result = staffs.Select(staff => (StaffDto)staff).ToList();
    }

    [EventHandler]
    public async Task GetStaffsByRoleAsync(StaffsByRoleQuery query)
    {
        var staffs = await _authDbContext.Set<Staff>()
                                         .Include(s => s.Position)
                                         .Include(staff => staff.DepartmentStaffs)
                                         .ThenInclude(departmentStaff => departmentStaff.Department)
                                         .Include(staff => staff.User)
                                         .ThenInclude(user => user.Roles)
                                         .Where(staff => staff.User.Roles.Any(role => role.RoleId == query.RoleId))
                                         .ToListAsync();

        query.Result = staffs.Select(staff => (StaffDto)staff).ToList();
    }

    [EventHandler]
    public async Task GetStaffTotalByDepartmentAsync(StaffTotalByDepartmentQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.DepartmentStaffs)
                                        .CountAsync(staff => staff.DepartmentStaffs.Any(department => department.DepartmentId == query.DepartmentId));

        query.Result = total;
    }

    [EventHandler]
    public async Task GetStaffTotalByTeamAsync(StaffTotalByTeamQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.TeamStaffs)
                                        .CountAsync(staff => staff.TeamStaffs.Any(team => team.TeamId == query.TeamId));

        query.Result = total;
    }

    [EventHandler]
    public async Task GetStaffTotalByRoleAsync(StaffTotalByRoleQuery query)
    {
        var total = await _authDbContext.Set<Staff>()
                                        .Include(staff => staff.User)
                                        .ThenInclude(user => user.Roles)
                                        .CountAsync(staff => staff.User.Roles.Any(role => role.RoleId == query.RoleId));

        query.Result = total;
    }

    #endregion

    #region ThirdPartyUser

    [EventHandler]
    public async Task GetThirdPartyUsersAsync(ThirdPartyUsersQuery query)
    {
        Expression<Func<ThirdPartyUser, bool>> condition = tpu => true;
        if (query.Enabled is not null)
            condition = condition.And(tpu => tpu.Enabled == query.Enabled);

        if (query.StartTime is not null)
            condition = condition.And(tpu => tpu.CreationTime >= query.StartTime);

        if (query.EndTime is not null)
            condition = condition.And(tpu => tpu.CreationTime <= query.EndTime);

        if (query.UserId != Guid.Empty)
            condition = condition.And(tpu => tpu.Id == query.UserId);

        var tpuQuery = _authDbContext.Set<ThirdPartyUser>().Where(condition);
        var total = await tpuQuery.LongCountAsync();
        var tpus = await tpuQuery.Include(tpu => tpu.CreateUser)
                                 .Include(tpu => tpu.ModifyUser)
                                 .OrderByDescending(s => s.ModificationTime)
                                 .ThenByDescending(s => s.CreationTime)
                                 .Skip((query.Page - 1) * query.PageSize)
                                 .Take(query.PageSize)
                                 .ToListAsync();

        query.Result = new(total, tpus.Select(tpu => (ThirdPartyUserDto)tpu).ToList());
    }

    [EventHandler]
    public async Task GetThirdPartyUserDetailAsync(ThirdPartyUserDetailQuery query)
    {
        var tpu = await _thirdPartyUserRepository.GetDetail(query.ThirdPartyUserId);
        if (tpu is null) throw new UserFriendlyException("This thirdPartyUser data does not exist");

        query.Result = tpu;
    }

    #endregion

    #region ThirdPartyIdp

    [EventHandler]
    public async Task GetThirdPartyIdpsAsync(ThirdPartyIdpsQuery query)
    {
        Expression<Func<ThirdPartyIdp, bool>> condition = user => true;

        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(thirdPartyIdp => thirdPartyIdp.Name.Contains(query.Search) || thirdPartyIdp.DisplayName.Contains(query.Search));

        var thirdPartyIdps = await _thirdPartyIdpRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(ThirdPartyIdp.ModificationTime)] = true,
                [nameof(ThirdPartyIdp.CreationTime)] = true,
            }
        });

        query.Result = new(thirdPartyIdps.Total, thirdPartyIdps.Result.Select(thirdPartyIdp => (ThirdPartyIdpDto)thirdPartyIdp).ToList());
    }

    [EventHandler]
    public async Task GetThirdPartyIdpDetailAsync(ThirdPartyIdpDetailQuery query)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == query.ThirdPartyIdpId);
        if (thirdPartyIdp is null) throw new UserFriendlyException("This thirdPartyIdp data does not exist");

        query.Result = thirdPartyIdp;
    }

    [EventHandler]
    public async Task GetLdapDetailDtoAsync(LdapDetailQuery query)
    {
        var thirdPartyIdp = await _ldapIdpRepository.FindAsync(ldap => ldap.Name == LdapConsts.LDAP_NAME);
        thirdPartyIdp?.Adapt(query.Result);
    }

    [EventHandler]
    public async Task GetThirdPartyIdpSelectAsync(ThirdPartyIdpSelectQuery query)
    {
        Expression<Func<ThirdPartyIdp, bool>> condition = ThirdPartyIdp => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(thirdPartyIdp => thirdPartyIdp.Name.Contains(query.Search) || thirdPartyIdp.DisplayName.Contains(query.Search));
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync();

        query.Result = thirdPartyIdps.Select(tpIdp => new ThirdPartyIdpSelectDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.ClientSecret, tpIdp.Url, tpIdp.Icon, tpIdp.VerifyType)).ToList();
    }

    #endregion

    #region Team

    [EventHandler]
    public async Task TeamListAsync(TeamListQuery teamListQuery)
    {
        Expression<Func<Team, bool>> condition = _ => true;
        if (!string.IsNullOrWhiteSpace(teamListQuery.Name))
        {
            condition = condition.And(t => t.Name.Contains(teamListQuery.Name));
        }
        if (teamListQuery.UserId != Guid.Empty)
        {
            var staffId = (await _authDbContext.Set<Staff>()
                                        .FirstOrDefaultAsync(staff => staff.UserId == teamListQuery.UserId))?.Id;
            if (staffId != default)
                condition = condition.And(t => t.TeamStaffs.Any(s => s.StaffId == staffId));
            else return;
        }
        var teams = await _teamRepository.GetListInCludeAsync(condition,
            tl => tl.OrderByDescending(t => t.ModificationTime), new List<string> { nameof(Team.TeamStaffs) });
        foreach (var team in teams.ToList())
        {
            var modifierName = _memoryCacheClient.Get<CacheUser>(CacheKey.UserKey(team.Modifier))?.DisplayName ?? "";
            var staffIds = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin)
                    .Select(s => s.StaffId);

            var adminAvatar = (await _staffRepository.GetListAsync(s => staffIds.Contains(s.Id))).Select(s => s.Avatar).ToList();

            teamListQuery.Result.Add(new TeamDto(team.Id, team.Name, team.Avatar.Url, team.Description, team.TeamStaffs.Count,
                adminAvatar, modifierName, team.ModificationTime));
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
                Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Admin).ToDictionary(p => p.PermissionId, p => p.Effect)
            },
            TeamMember = new TeamPersonnelDto
            {
                Staffs = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Member).Select(s => s.StaffId).ToList(),
                Roles = team.TeamRoles.Where(r => r.TeamMemberType == TeamMemberTypes.Member).Select(r => r.RoleId).ToList(),
                Permissions = team.TeamPermissions.Where(p => p.TeamMemberType == TeamMemberTypes.Member).ToDictionary(p => p.PermissionId, p => p.Effect)
            }
        };
    }

    [EventHandler]
    public async Task TeamDetailForExternalAsync(TeamDetailForExternalQuery query)
    {
        var team = await _authDbContext.Set<Team>()
                                       .Include(t => t.TeamStaffs)
                                       .ThenInclude(ts => ts.Staff)
                                       .AsSplitQuery()
                                       .FirstOrDefaultAsync(t => t.Id == query.TeamId);

        if (team is null) throw new UserFriendlyException("This team data does not exist");
        query.Result = new TeamDetailForExternalDto
        {
            Id = team.Id,
            Name = team.Name,
            Description = team.Description,
            TeamType = team.TeamType,
            Avatar = team.Avatar.Url,
            TeamAdmin = team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Admin).Select(ts => (StaffDto)ts.Staff).ToList(),
            TeamMember = team.TeamStaffs.Where(ts => ts.TeamMemberType == TeamMemberTypes.Member).Select(ts => (StaffDto)ts.Staff).ToList(),
        };
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

    #endregion

    [EventHandler]
    public async Task UserVisitedListQueryAsync(UserVisitedListQuery userVisitedListQuery)
    {
        var key = CacheKey.UserVisitKey(userVisitedListQuery.UserId);
        var visited = await _memoryCacheClient.GetAsync<List<string>>(key);
        if (visited != null)
        {
            var menus = _authDbContext.Set<Permission>().Where(p => visited.Contains(p.Url))
                .Select(p => new
                {
                    p.Name,
                    p.Url
                }).ToDictionary(p => p.Url, p => p.Name);
            userVisitedListQuery.Result = visited.Select(v => new UserVisitedDto
            {
                Url = v,
                Name = menus.ContainsKey(v) ? menus[v] : ""
            }).Where(v => !string.IsNullOrEmpty(v.Name)).ToList();
        }
    }

    [EventHandler]
    public async Task UserSystemBizDataQueryAsync(UserSystemBusinessDataQuery userSystemBusinessData)
    {
        userSystemBusinessData.Result = await _memoryCacheClient.GetAsync<string>(
            CacheKey.UserSystemDataKey(userSystemBusinessData.UserId, userSystemBusinessData.SystemId)) ?? "";
    }
}
