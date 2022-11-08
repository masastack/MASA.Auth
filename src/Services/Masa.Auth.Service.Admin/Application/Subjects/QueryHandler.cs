﻿// Copyright (c) MASA Stack All rights reserved.
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
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IPmClient _pmClient;

    public QueryHandler(
        IUserRepository userRepository,
        ITeamRepository teamRepository,
        IStaffRepository staffRepository,
        IThirdPartyUserRepository thirdPartyUserRepository,
        IThirdPartyIdpRepository thirdPartyIdpRepository,
        ILdapIdpRepository ldapIdpRepository,
        AuthDbContext authDbContext,
        IAutoCompleteClient autoCompleteClient,
        IMultilevelCacheClient multilevelCacheClient,
        IPmClient pmClient)
    {
        _userRepository = userRepository;
        _teamRepository = teamRepository;
        _staffRepository = staffRepository;
        _thirdPartyUserRepository = thirdPartyUserRepository;
        _thirdPartyIdpRepository = thirdPartyIdpRepository;
        _ldapIdpRepository = ldapIdpRepository;
        _authDbContext = authDbContext;
        _autoCompleteClient = autoCompleteClient;
        _multilevelCacheClient = multilevelCacheClient;
        _pmClient = pmClient;
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
        var creator = await _multilevelCacheClient.GetAsync<CacheUser>(CacheKey.UserKey(user.Creator));
        var modifier = await _multilevelCacheClient.GetAsync<CacheUser>(CacheKey.UserKey(user.Modifier));
        var userDetail = await UserSplicingDataAsync(user);
        query.Result = userDetail!;
        query.Result.Creator = creator?.DisplayName;
        query.Result.Modifier = modifier?.DisplayName;
    }

    [EventHandler]
    public async Task FindUserByAccountAsync(FindUserByAccountQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(user => user.Account == query.Account, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}"
        });
        query.Result = await UserSplicingDataAsync(user);
    }

    [EventHandler]
    public async Task GetUsersByAccountAsync(UsersByAccountQuery query)
    {
        var users = await _userRepository.GetListAsync(u => query.Accounts.Contains(u.Account));
        query.Result = users.Adapt<List<UserSimpleModel>>();
    }

    [EventHandler]
    public async Task GetUserByPhoneAsync(UserByPhoneQuery query)
    {
        var user = await _userRepository.FindAsync(u => u.PhoneNumber == query.PhoneNumber);
        query.Result = user?.Adapt<UserSimpleModel>();
    }

    [EventHandler]
    public async Task FindUserByEmailAsync(FindUserByEmailQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(user => user.Email == query.Email, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}"
        });
        query.Result = await UserSplicingDataAsync(user);
    }

    [EventHandler]
    public async Task FindUserByPhoneNumberAsync(FindUserByPhoneNumberQuery query)
    {
        var user = await _userRepository.FindWithIncludAsync(user => user.PhoneNumber == query.PhoneNumber, new List<string> {
            $"{nameof(User.Roles)}.{nameof(UserRole.Role)}"
        });
        query.Result = await UserSplicingDataAsync(user);
    }

    async Task<UserDetailDto?> UserSplicingDataAsync(User? user)
    {
        UserDetailDto? userDetailDto = null;
        if (user != null)
        {
            userDetailDto = user;
            var staff = await _multilevelCacheClient.GetAsync<CacheStaff>(CacheKey.StaffKey(user.Id));
            userDetailDto.StaffId = staff?.Id;
            userDetailDto.StaffDisplayName = staff?.DisplayName;
            userDetailDto.CurrentTeamId = staff?.CurrentTeamId;
        }
        return userDetailDto;
    }

    [EventHandler]
    public async Task GetUserSelectAsync(UserSelectQuery query)
    {
        var response = await _autoCompleteClient.GetBySpecifyDocumentAsync<UserSelectDto>(
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
    public async Task UserPortraitsAsync(UserPortraitsQuery userPortraitsQuery)
    {
        foreach (var userId in userPortraitsQuery.UserIds)
        {
            var userCache = await _multilevelCacheClient.GetAsync<CacheUser>(CacheKey.UserKey(userId));
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

    [EventHandler]
    public async Task HasPasswordAsync(HasPasswordQuery query)
    {
        query.Result = await _authDbContext.Set<User>().AnyAsync(user => user.Id == query.UserId && user.Password != null && user.Password != "");
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
            condition = condition.And(staff =>
                staff.DisplayName.Contains(query.Search) ||
                staff.PhoneNumber.Contains(query.Search) ||
                staff.JobNumber.Contains(query.Search) ||
                staff.Position!.Name.Contains(query.Search) ||
                staff.User.Account.Contains(query.Search));

        if (query.DepartmentId != Guid.Empty)
        {
            var staffIds = _authDbContext.Set<DepartmentStaff>()
                .Where(ds => ds.DepartmentId == query.DepartmentId)
                .Select(ds => ds.StaffId);
            condition = condition.And(s => staffIds.Contains(s.Id));
        }
        var staffQuery = _authDbContext.Set<Staff>()
                                       .Include(s => s.User)
                                       .Include(s => s.DepartmentStaffs)
                                       .ThenInclude(ds => ds.Department)
                                       .Include(s => s.Position)
                                       .Where(condition);
        var total = await staffQuery.LongCountAsync();
        var staffs = await staffQuery.OrderByDescending(s => s.ModificationTime)
                                     .ThenByDescending(s => s.CreationTime)
                                     .Skip((query.Page - 1) * query.PageSize)
                                     .Take(query.PageSize)
                                     .ToListAsync();

        query.Result = new(total, staffs.Select(staff =>
        {
            var staffDto = (StaffDto)staff;
            staffDto.Account = staff.User.Account;
            return staffDto;
        }).ToList());
    }

    [EventHandler]
    public async Task GetStaffDetailAsync(StaffDetailQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.User)
                                        .Include(s => s.DepartmentStaffs)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .FirstOrDefaultAsync(s => s.Id == query.StaffId);
        if (staff is null) throw new UserFriendlyException("This staff data does not exist");

        query.Result = staff;

        var (creator, modifier) = await _multilevelCacheClient.GetActionInfoAsync(staff.Creator, staff.Modifier);
        query.Result.Creator = creator;
        query.Result.Modifier = modifier;
    }

    [EventHandler]
    public async Task GetStaffDetailByUserIdAsync(StaffDetailByUserIdQuery query)
    {
        var staff = await _authDbContext.Set<Staff>()
                                        .Include(s => s.User)
                                        .ThenInclude(user => user.Roles)
                                        .ThenInclude(ur => ur.Role)
                                        .Include(s => s.DepartmentStaffs)
                                        .ThenInclude(ds => ds.Department)
                                        .Include(s => s.TeamStaffs)
                                        .Include(s => s.Position)
                                        .AsSplitQuery()
                                        .FirstOrDefaultAsync(s => s.UserId == query.UserId);
        if (staff is not null)
        {
            var staffDetailModel = staff.Adapt<StaffDetailModel>();
            staffDetailModel.Department = staff.DepartmentStaffs.FirstOrDefault()?.Department?.Name ?? "";
            staffDetailModel.Position = staff.Position?.Name ?? "";
            staffDetailModel.Roles = staff.User.Roles.Select(ur => ur.Role).Adapt<List<RoleModel>>();
            staffDetailModel.Teams = new();
            query.Result = staffDetailModel;
        }
    }

    [EventHandler]
    public async Task GetStaffSelectAsync(StaffSelectQuery query)
    {
        Expression<Func<Staff, bool>> condition = staff => true;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(s => s.DisplayName.Contains(query.Search) || s.Name.Contains(query.Search) || s.JobNumber.Contains(query.Search));
        var staffs = await _staffRepository.GetListAsync(condition);

        query.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.DisplayName, s.Avatar)).ToList();
    }

    [EventHandler]
    public async Task GetStaffSelectByIdsAsync(StaffSelectByIdQuery staffSelectByIdQuery)
    {
        var staffs = await _staffRepository.GetListAsync(s => staffSelectByIdQuery.Ids.Contains(s.Id));
        staffSelectByIdQuery.Result = staffs.Select(s => new StaffSelectDto(s.Id, s.JobNumber, s.Name, s.DisplayName, s.Avatar)).ToList();
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

    [EventHandler]
    public async Task GetDefaultPasswordAsync(StaffDefaultPasswordQuery query)
    {
        var defaultPasswordDto = await _multilevelCacheClient.GetAsync<StaffDefaultPasswordDto>(CacheKey.STAFF_DEFAULT_PASSWORD);
        query.Result = defaultPasswordDto ?? new();
    }

    #endregion

    #region ThirdPartyUser

    [EventHandler]
    public async Task GetThirdPartyUsersAsync(ThirdPartyUsersQuery query)
    {
        Expression<Func<ThirdPartyUser, bool>> condition = tpu => true;
        condition = condition.And(query.Enabled is not null, tpu => tpu.Enabled == query.Enabled)
                             .And(query.StartTime is not null, tpu => tpu.CreationTime >= query.StartTime)
                             .And(query.EndTime is not null, tpu => tpu.CreationTime <= query.EndTime)
                             .And(query.ThirdPartyId is not null, tpu => tpu.ThirdPartyIdpId == query.ThirdPartyId)
                             .And(string.IsNullOrEmpty(query.Search) is false, tpu => tpu.User.DisplayName.Contains(query.Search!));

        var tpuQuery = _authDbContext.Set<ThirdPartyUser>().Where(condition);
        var total = await tpuQuery.LongCountAsync();
        var tpus = await tpuQuery.Include(tpu => tpu.User)
                                 .OrderByDescending(s => s.ModificationTime)
                                 .ThenByDescending(s => s.CreationTime)
                                 .Skip((query.Page - 1) * query.PageSize)
                                 .Take(query.PageSize)
                                 .ToListAsync();

        query.Result = new(total, tpus.Select(tpu =>
        {
            var dto = (ThirdPartyUserDto)tpu;
            var (creator, modifier) = _multilevelCacheClient.GetActionInfoAsync(tpu.Creator, tpu.Modifier).Result;
            dto.Creator = creator;
            dto.Modifier = modifier;
            return dto;
        }).ToList());
    }

    [EventHandler]
    public async Task GetThirdPartyUserDetailAsync(ThirdPartyUserDetailQuery query)
    {
        var tpu = await _thirdPartyUserRepository.GetDetail(query.ThirdPartyUserId);
        if (tpu is null) throw new UserFriendlyException("This thirdPartyUser data does not exist");

        query.Result = tpu;

        var (creator, modifier) = await _multilevelCacheClient.GetActionInfoAsync(tpu.Creator, tpu.Modifier);
        query.Result.User.Creator = creator;
        query.Result.User.Modifier = modifier;
    }

    [EventHandler]
    public async Task GetThirdPartyUserAsync(ThirdPartyUserQuery query)
    {
        var tpUser = await _authDbContext.Set<ThirdPartyUser>()
                                         .Include(tpu => tpu.User)
                                         .FirstOrDefaultAsync(tpu => tpu.ThridPartyIdentity == query.ThridPartyIdentity);
        query.Result = tpUser?.User?.Adapt<UserModel>();
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

        query.Result = new(thirdPartyIdps.Total, thirdPartyIdps.Result.Select(thirdPartyIdp => thirdPartyIdp.Adapt<ThirdPartyIdpDto>()).ToList());
    }

    [EventHandler]
    public async Task GetAllThirdPartyIdpAsync(AllThirdPartyIdpQuery query)
    {
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync(tpIdp => tpIdp.Enabled);
        query.Result = thirdPartyIdps.Adapt<List<ThirdPartyIdpModel>>();
    }

    [EventHandler]
    public async Task GetThirdPartyIdpDetailAsync(ThirdPartyIdpDetailQuery query)
    {
        var thirdPartyIdp = await _thirdPartyIdpRepository.FindAsync(thirdPartyIdp => thirdPartyIdp.Id == query.ThirdPartyIdpId);
        if (thirdPartyIdp is null) throw new UserFriendlyException("This thirdPartyIdp data does not exist");

        query.Result = thirdPartyIdp.Adapt<ThirdPartyIdpDetailDto>();
    }

    [EventHandler]
    public async Task GetIdentityProviderByTypeAsync(IdentityProviderByTypeQuery query)
    {
        var identityProvider = await _authDbContext.Set<IdentityProvider>()
                                                       .FirstOrDefaultAsync(ip => ip.ThirdPartyIdpType == query.ThirdPartyIdpType);

        query.Result = identityProvider ?? throw new UserFriendlyException($"IdentityProvider {query.ThirdPartyIdpType} not exist");
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

        query.Result = thirdPartyIdps.Select(tpIdp => new ThirdPartyIdpSelectDto(tpIdp.Id, tpIdp.Name, tpIdp.DisplayName, tpIdp.ClientId, tpIdp.ClientSecret, tpIdp.Icon, tpIdp.AuthenticationType)).ToList();
    }

    [EventHandler]
    public async Task GetExternalThirdPartyIdpsAsync(ExternalThirdPartyIdpsQuery query)
    {
        var thirdPartyIdps = await _thirdPartyIdpRepository.GetListAsync();
        query.Result = LocalAuthenticationDefaultsProvider.GetAll()
                                                     .Where(adp => thirdPartyIdps.Any(tpIdp => tpIdp.ThirdPartyIdpType.ToString() == adp.Scheme) is false)
                                                     .Adapt<List<ThirdPartyIdpModel>>()
                                                     .ToList();
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
                                        .Include(team => team.TeamRoles)
                                        .ThenInclude(tr => tr.Role)
                                        .Where(condition)
                                        .AsSplitQuery()
                                        .OrderByDescending(t => t.ModificationTime)
                                        .ToListAsync();

        foreach (var team in teams.ToList())
        {
            var modifierName = _multilevelCacheClient.Get<CacheUser>(CacheKey.UserKey(team.Modifier))?.DisplayName ?? "";
            var staffIds = team.TeamStaffs.Where(s => s.TeamMemberType == TeamMemberTypes.Admin)
                    .Select(s => s.StaffId);

            var adminAvatar = (await _staffRepository.GetListAsync(s => staffIds.Contains(s.Id))).Select(s => s.Avatar).ToList();
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
        Expression<Func<Team, bool>> teamStaffCondition = _ => true;
        if (query.UserId != default)
        {
            teamStaffCondition = teamStaffCondition.And(team => team.TeamStaffs.Any(ts => ts.UserId == query.UserId));
        }
        var teams = await _authDbContext.Set<Team>()
                                        .Where(condition)
                                        .Include(team => team.TeamStaffs)
                                        .Where(teamStaffCondition)
                                        .Include(team => team.TeamRoles)
                                        .ThenInclude(tr => tr.Role)
                                        .ToListAsync();
        foreach (var team in teams)
        {
            var roles = team.TeamRoles
                            .Where(tr => tr.TeamMemberType == TeamMemberTypes.Member)
                            .Select(tr => new RoleSelectDto(tr.Role.Id, tr.Role.Name, tr.Role.Code, tr.Role.Limit, tr.Role.AvailableQuantity))
                            .ToList();
            query.Result.Add(new TeamRoleSelectDto(team.Id, team.Name, team.Avatar.Url, roles));
        }
    }

    [EventHandler]
    public async Task GetTeamByUserAsync(TeamByUserQuery query)
    {
        var teams = await _authDbContext.Set<TeamStaff>()
                                        .Where(ts => ts.UserId == query.UserId)
                                        .ToListAsync();

        query.Result = teams.Select(team => new TeamSampleDto(team.TeamId, team.TeamMemberType))
                            .ToList();
    }

    #endregion

    [EventHandler]
    public async Task UserVisitedListQueryAsync(UserVisitedListQuery userVisitedListQuery)
    {
        var key = CacheKey.UserVisitKey(userVisitedListQuery.UserId);
        var visited = await _multilevelCacheClient.GetAsync<List<CacheUserVisited>>(key);
        if (visited != null)
        {
            var apps = await _pmClient.AppService.GetListAsync();
            //todo cache
            var menus = visited.GroupJoin(_authDbContext.Set<Permission>().Where(p => p.Type == PermissionTypes.Menu).AsEnumerable(),
                v => new { v.AppId, Url = v.Url.ToLower().Trim('/') }, p => new { p.AppId, Url = p.Url?.ToLower().Trim('/') ?? "" }, (v, p) => new
                {
                    UserVisited = v,
                    Permissions = p
                }).SelectMany(x => x.Permissions.DefaultIfEmpty(), (v, p) => new
                {
                    v.UserVisited,
                    Permission = p
                })
                .GroupJoin(apps, p => p.Permission?.AppId, a => a.Identity, (p, a) => new
                {
                    Permission = p.Permission,
                    AppModels = a
                }).SelectMany(x => x.AppModels.DefaultIfEmpty(), (p, a) =>
                   new KeyValuePair<string, string>($"{a?.Url.TrimEnd('/')}/{p.Permission?.Url.TrimStart('/')}", p.Permission?.Name ?? "")
                ).ToList();
            userVisitedListQuery.Result = menus.Select(v => new UserVisitedModel
            {
                Url = v.Key,
                Name = v.Value
            }).Where(v => !string.IsNullOrEmpty(v.Name)).ToList();
        }
    }

    [EventHandler]
    public async Task UserSystemBizDataQueryAsync(UserSystemBusinessDataQuery userSystemBusinessData)
    {
        userSystemBusinessData.Result = await _multilevelCacheClient.GetAsync<string>(
            CacheKey.UserSystemDataKey(userSystemBusinessData.UserId, userSystemBusinessData.SystemId)) ?? "";
    }
}
