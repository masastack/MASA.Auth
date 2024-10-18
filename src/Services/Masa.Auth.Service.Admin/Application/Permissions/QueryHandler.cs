// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class QueryHandler
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly AuthDbContext _authDbContext;
    private readonly UserDomainService _userDomainService;
    private readonly IDistributedCacheClient _cacheClient;
    private readonly IEventBus _eventBus;
    private readonly ILogger<QueryHandler> _logger;
    private readonly OperaterProvider _operaterProvider;

    public QueryHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        IDistributedCacheClient cacheClient,
        IEventBus eventBus,
        ILogger<QueryHandler> logger,
        OperaterProvider operaterProvider)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _cacheClient = cacheClient;
        _eventBus = eventBus;
        _logger = logger;
        _operaterProvider = operaterProvider;
    }

    #region Role

    [EventHandler]
    public async Task GetRoleListAsync(GetRolesQuery query)
    {
        Expression<Func<Role, bool>> condition = role => true;
        if (query.Enabled is not null)
            condition = condition.And(role => role.Enabled == query.Enabled);
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(user => user.Name.Contains(query.Search));

        var roleQuery = _authDbContext.Set<Role>().Where(condition);
        var total = await roleQuery.LongCountAsync();
        var roles = await roleQuery.OrderByDescending(s => s.ModificationTime)
                                   .ThenByDescending(s => s.CreationTime)
                                   .Skip((query.Page - 1) * query.PageSize)
                                   .Take(query.PageSize)
                                   .ToListAsync();

        query.Result = new(total, roles.Select(role =>
        {
            var dto = (RoleDto)role;
            var (creator, modifier) = _operaterProvider.GetActionInfoAsync(role.Creator, role.Modifier).Result;
            dto.Creator = creator;
            dto.Modifier = modifier;
            return dto;
        }).ToList());
    }

    [EventHandler]
    public async Task GetRoleDetailAsync(RoleDetailQuery query)
    {
        var role = await _roleRepository.GetDetailAsync(query.RoleId);
        if (role is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);

        query.Result = role;
        var (creator, modifier) = await _operaterProvider.GetActionInfoAsync(role.Creator, role.Modifier);
        query.Result.Creator = creator;
        query.Result.Modifier = modifier;
    }

    [EventHandler]
    public async Task GetDetailExternalAsync(RoleDetailExternalQuery query)
    {
        var role = await _roleRepository.GetByIdAsync(query.RoleId);
        var result = new RoleSimpleDetailDto
        {
            Id = role.Id,
            Name = role.Name,
            Code = role.Code,
            Enabled = role.Enabled
        };
        var childrenIds = role.ChildrenRoles.Select(x => x.RoleId);
        result.Children = (await _roleRepository.GetListAsync(r => childrenIds.Contains(r.Id))).Select(r => new RoleSimpleDetailDto
        {
            Id = r.Id,
            Name = r.Name,
            Code = r.Code,
            Enabled = r.Enabled
        }).ToList();
        query.Result = result;
    }

    [EventHandler]
    public async Task GetRoleOwnerAsync(RoleOwnerQuery query)
    {
        var role = await _authDbContext.Set<Role>()
                            .Include(r => r.Users)
                            .ThenInclude(ur => ur.User)
                            .Include(r => r.Teams)
                            .ThenInclude(tr => tr.Team)
                            .AsSplitQuery()
                            .FirstOrDefaultAsync(r => r.Id == query.RoleId);
        if (role is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);

        query.Result = new(
            role.Users.Select(ur => new UserSelectDto(ur.User.Id, ur.User.Name, ur.User.DisplayName, ur.User.Account, ur.User.PhoneNumber, ur.User.Email, ur.User.Avatar)).ToList(),
            role.Teams.DistinctBy(team => team.TeamId).Select(tr => new TeamSelectDto(tr.Team.Id, tr.Team.Name, tr.Team.Avatar.Url)).ToList()
        );
    }

    [EventHandler]
    public async Task GetTopRoleSelectAsync(TopRoleSelectQuery query)
    {
        var roleSelect = await _authDbContext.Set<RoleRelation>()
                                    .Include(r => r.ParentRole)
                                    .Where(r => r.RoleId == query.RoleId)
                                    .Select(r => new RoleSelectDto(r.ParentRole.Id, r.ParentRole.Name, r.ParentRole.Code, r.ParentRole.Limit, r.ParentRole.AvailableQuantity))
                                    .ToListAsync();

        query.Result = roleSelect;
    }

    [EventHandler]
    public async Task GetRoleSelectForUserAsync(RoleSelectForUserQuery query)
    {
        query.Result = await GetRoleSelectAsync();
    }

    [EventHandler]
    public async Task GetRoleSelectForRoleAsync(RoleSelectForRoleQuery query)
    {
        var roleSelect = await GetRoleSelectAsync();
        if (query.RoleId != Guid.Empty)
        {
            var roleRelations = await _authDbContext.Set<RoleRelation>().ToListAsync();
            var parentRoles = FindParentRoles(roleRelations, query.RoleId);
            roleSelect = roleSelect.Where(r => parentRoles.Contains(r.Id) is false).ToList();
        }
        query.Result = roleSelect;

        List<Guid> FindParentRoles(List<RoleRelation> roleRelations, Guid roleId)
        {
            var parentRoles = new List<Guid>
            {
                roleId
            };
            foreach (var roleRelation in roleRelations)
            {
                if (roleRelation.RoleId == roleId)
                    parentRoles.AddRange(FindParentRoles(roleRelations, roleRelation.ParentId));
            }

            return parentRoles;
        }
    }

    [EventHandler]
    public async Task GetRoleSelectForTeamAsync(RoleSelectForTeamQuery query)
    {
        query.Result = await GetRoleSelectAsync();
    }

    [EventHandler]
    public async Task GetRoleSelectAsync(RoleSelectQuery query)
    {
        var roleSelect = await _authDbContext.Set<Role>()
                                         .Where(r => r.Name.Contains(query.Name))
                                         .Select(r => new RoleSelectDto(r.Id, r.Name, r.Code, r.Limit, r.AvailableQuantity))
                                         .ToListAsync();
        query.Result = roleSelect;
    }

    [EventHandler]
    public async Task GetUsersAsync(RoleUsersQuery query)
    {
        Expression<Func<UserRole, bool>> condition = x => x.RoleId == query.RoleId;

        var userRoleQuery = _authDbContext.Set<UserRole>().Include(x => x.User).Where(condition);
        var total = await userRoleQuery.LongCountAsync();
        var users = await userRoleQuery.OrderByDescending(s => s.ModificationTime)
                                   .ThenByDescending(s => s.CreationTime)
                                   .Skip((query.Page - 1) * query.PageSize)
                                   .Take(query.PageSize)
                                   .Select(x => x.User)
                                   .ToListAsync();
        var dtos = users.Adapt<List<UserSelectModel>>();
        query.Result = new(total, dtos);
    }


    private async Task<List<RoleSelectDto>> GetRoleSelectAsync()
    {
        var roleSelect = await _authDbContext.Set<Role>()
                                        .Select(r => new RoleSelectDto(r.Id, r.Name, r.Code, r.Limit, r.AvailableQuantity))
                                        .ToListAsync();
        return roleSelect;
    }

    #endregion 

    #region Permission

    [EventHandler]
    public void PermissionTypesQueryAsync(PermissionTypesQuery permissionTypesQuery)
    {
        permissionTypesQuery.Result = EnumUtil.GetList<PermissionTypes>().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Name,
            Value = pt.Value
        }).ToList();
    }

    [EventHandler]
    public async Task ApplicationPermissionsQueryAsync(ApplicationPermissionsQuery applicationPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == applicationPermissionsQuery.SystemId);

        applicationPermissionsQuery.Result = GetChildrenPermissions(Guid.Empty, permissions);
    }

    private List<AppPermissionDto> GetChildrenPermissions(Guid parentId, IEnumerable<Permission> all)
    {
        return all.Where(p => p.GetParentId() == parentId)
            .OrderBy(p => p.Order)
            .Select(p => new AppPermissionDto
            {
                AppId = p.AppId,
                PermissionId = p.Id,
                PermissionName = p.Name,
                Type = p.Type,
                Children = GetChildrenPermissions(p.Id, all)
            }).ToList();
    }

    [EventHandler]
    public async Task ApiPermissionSelectQueryAsync(ApiPermissionSelectQuery apiPermissionSelectQuery)
    {
        Expression<Func<Permission, bool>> condition = permission => permission.Type == PermissionTypes.Api;
        condition = condition.And(!string.IsNullOrEmpty(apiPermissionSelectQuery.SystemId), permission => permission.SystemId == apiPermissionSelectQuery.SystemId);

        var permissions = await _permissionRepository.GetPaginatedListAsync(condition, 0, apiPermissionSelectQuery.MaxCount);
        apiPermissionSelectQuery.Result = permissions
            .OrderBy(p => p.Order)
            .Select(p => new SelectItemDto<Guid>
            {
                Value = p.Id,
                Text = p.Name
            }).ToList();
    }

    [EventHandler]
    public async Task PermissionDetailQueryAsync(MenuPermissionDetailQuery menuPermissionDetailQuery)
    {
        var permission = await _permissionRepository.GetByIdAsync(menuPermissionDetailQuery.PermissionId);
        if (permission.Type == PermissionTypes.Api)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSIION_API_TYPE);
        }
        menuPermissionDetailQuery.Result = new MenuPermissionDetailDto
        {
            Name = permission.Name,
            Description = permission.Description,
            Icon = permission.Icon,
            MatchPattern = permission.MatchPattern,
            Code = permission.Code,
            Url = permission.Url,
            Type = permission.Type,
            Id = permission.Id,
            Enabled = permission.Enabled,
            ParentId = permission.GetParentId(),
            AppId = permission.AppId,
            Order = permission.Order,
            ApiPermissions = permission.AffiliationPermissionRelations.Select(pr => pr.AffiliationPermissionId).ToList(),
            Roles = permission.RolePermissions.Where(rp => rp.Effect).Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name, rp.Role.Code, rp.Role.Limit, rp.Role.AvailableQuantity)).ToList(),
            Teams = permission.TeamPermissions.Where(rp => rp.Effect).DistinctBy(e => e.TeamId).Select(tp => new TeamSelectDto(tp.Team.Id, tp.Team.Name, tp.Team.Avatar.Url)).ToList(),
            Users = permission.UserPermissions.Where(rp => rp.Effect).Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.DisplayName,
                Avatar = up.User.Avatar
            }).ToList()
        };
    }

    [EventHandler]
    public async Task PermissionDetailQueryAsync(ApiPermissionDetailQuery apiPermissionDetailQuery)
    {
        var permission = await _permissionRepository.FindAsync(apiPermissionDetailQuery.PermissionId)
                ?? throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSIION_NOT_FOUND);
        if (permission.Type != PermissionTypes.Api)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.PERMISSIION_NOT_API_TYPE);
        }
        apiPermissionDetailQuery.Result = new ApiPermissionDetailDto
        {
            Name = permission.Name,
            Description = permission.Description,
            Icon = permission.Icon,
            Code = permission.Code,
            Url = permission.Url,
            Type = permission.Type,
            Id = permission.Id,
            AppId = permission.AppId,
            Order = permission.Order
        };
    }

    [EventHandler]
    public async Task AppMenuListQueryAsync(AppMenuListQuery appMenuListQuery)
    {
        var userPermissionIds = await _userDomainService.GetPermissionIdsAsync(appMenuListQuery.UserId);
        var menus = await _cacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (menus == null || menus.Count < 1)
        {
            menus = (await _permissionRepository.GetListAsync(p => p.AppId == appMenuListQuery.AppId
                                && p.Type == PermissionTypes.Menu && userPermissionIds.Contains(p.Id) && p.Enabled)).Adapt<List<CachePermission>>();
        }
        menus = menus.Where(p => p.AppId == appMenuListQuery.AppId && p.Type == PermissionTypes.Menu && userPermissionIds.Contains(p.Id) && p.Enabled).ToList();
        appMenuListQuery.Result = GetMenus(menus, Guid.Empty);

        List<MenuModel> GetMenus(List<CachePermission> allMenus, Guid parentId)
        {
            return allMenus.Where(m => m.ParentId == parentId && m.Id != Guid.Empty)
                .OrderBy(m => m.Order)
                .Select(m => new MenuModel
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Url = m.Url.EnsureLeadingSlash(),
                    Icon = m.Icon,
                    MatchPattern = m.MatchPattern,
                    Children = GetMenus(allMenus, m.Id)
                }).ToList();
        }
    }

    [EventHandler]
    public async Task AppElementPermissionListQueryAsync(AppElementPermissionCodeListQuery appElementPermissionCodeListQuery)
    {
        var userPermissionIds = await _userDomainService.GetPermissionIdsAsync(appElementPermissionCodeListQuery.UserId);
        var elements = await _permissionRepository.GetListAsync(p => p.AppId == appElementPermissionCodeListQuery.AppId
                            && p.Type == PermissionTypes.Element && userPermissionIds.Contains(p.Id) && p.Enabled);
        appElementPermissionCodeListQuery.Result = elements.Select(e => e.Code).ToList();
    }

    [EventHandler]
    public async Task AppPermissionListQueryAsync(AppPermissionCodeListQuery appPermissionCodeListQuery)
    {
        var userPermissionIds = await _userDomainService.GetPermissionIdsAsync(appPermissionCodeListQuery.UserId);
        var elements = await _permissionRepository.GetListAsync(p => p.AppId == appPermissionCodeListQuery.AppId
                            && userPermissionIds.Contains(p.Id) && p.Enabled);
        appPermissionCodeListQuery.Result = elements.Select(e => e.Code).ToList();
    }

    [EventHandler]
    public async Task AppPermissionAuthorizedQueryAsync(AppPermissionAuthorizedQuery appPermissionAuthorizedQuery)
    {
        appPermissionAuthorizedQuery.Result = await _userDomainService.AuthorizedAsync(appPermissionAuthorizedQuery.AppId,
                    appPermissionAuthorizedQuery.Code, appPermissionAuthorizedQuery.UserId);
    }

    [EventHandler]
    public async Task GetPermissionsByRoleAsync(PermissionsByRoleQuery query)
    {
        if (query.Roles.Count == 0)
        {
            return;
        }

        var cacheRolePermissions = await GetPermissionsByCacheAsync(query.Roles);
        if (cacheRolePermissions == null || cacheRolePermissions.Count < 1)
        {
            cacheRolePermissions = await GetPermissionsAsync(query.Roles);
        }
        query.Result = cacheRolePermissions;

        async Task<List<Guid>> GetPermissionsAsync(IEnumerable<Guid> roleIds)
        {
            roleIds = roleIds.Distinct().ToList();
            var permissions = new List<Guid>();
            var rolePermissions = await _authDbContext.Set<Role>()
                                          .Where(r => r.Enabled == true && roleIds.Contains(r.Id))
                                          .Include(r => r.ChildrenRoles)
                                          .Include(r => r.Permissions)
                                          .Select(r => new { r.Permissions, r.ChildrenRoles })
                                          .ToListAsync();
            foreach (var rolePermission in rolePermissions)
            {
                var childPermissions = new List<Guid>();
                if (rolePermission.ChildrenRoles.Any())
                {
                    childPermissions = await GetPermissionsAsync(rolePermission.ChildrenRoles.Select(cr => cr.RoleId));
                    var rejectPermissions = rolePermission.Permissions.Where(p => p.Effect is false).ToList();
                    childPermissions = childPermissions.Where(p => rejectPermissions.All(rp => rp.PermissionId != p)).ToList();
                    childPermissions.AddRange(rolePermission.Permissions.Where(p => p.Effect).Select(p => p.PermissionId));
                    permissions.AddRange(childPermissions);
                }
                childPermissions.AddRange(rolePermission.Permissions.Where(p => p.Effect).Select(p => p.PermissionId));
                permissions.AddRange(childPermissions);
            }

            return permissions.Distinct().ToList();
        }

        async Task<List<Guid>> GetPermissionsByCacheAsync(IEnumerable<Guid> roleIds)
        {
            roleIds = roleIds.Distinct().ToList();
            var permissions = new List<Guid>();
            var roleCacheKeys = roleIds.Select(e => CacheKey.RoleKey(e));
            var rolePermissions = await _cacheClient.GetListAsync<CacheRole>(roleCacheKeys);
            foreach (var rolePermission in rolePermissions)
            {
                if (rolePermission == null)
                {
                    continue;
                }
                var childPermissions = new List<Guid>();
                if (rolePermission.ChildrenRoles != null && rolePermission.ChildrenRoles.Any())
                {
                    childPermissions = await GetPermissionsByCacheAsync(rolePermission.ChildrenRoles);
                    var rejectPermissions = rolePermission.Permissions.Where(p => p.Effect is false).ToList();
                    childPermissions = childPermissions.Where(p => rejectPermissions.All(rp => rp.PermissionId != p)).ToList();
                    childPermissions.AddRange(rolePermission.Permissions.Where(p => p.Effect).Select(p => p.PermissionId));
                    permissions.AddRange(childPermissions);
                }
                childPermissions.AddRange(rolePermission.Permissions.Where(p => p.Effect).Select(p => p.PermissionId));
                permissions.AddRange(childPermissions);
            }
            return permissions.Distinct().ToList();
        }
    }

    [EventHandler]
    public async Task GetPermissionsByTeamAsync(PermissionsByTeamQuery query)
    {
        if (query.Teams.Count == 0) return;
        var teamIds = query.Teams.Select(team => team.Id).ToList();

        var permissionIds = await GetPermissionsAsync(teamIds);
        query.Result.AddRange(permissionIds);

        async Task<List<Guid>> GetPermissionsAsync(IEnumerable<Guid> teamEnumerableIds)
        {
            var permissionIds = new List<Guid>();
            var teamRoles = await _authDbContext.Set<TeamRole>()
                                           .Where(tr => teamEnumerableIds.Contains(tr.TeamId))
                                           .ToListAsync();
            var teamPermissions = await _authDbContext.Set<TeamPermission>()
                                                     .Where(tp => teamEnumerableIds.Contains(tp.TeamId))
                                                     .ToListAsync();
            foreach (var team in query.Teams)
            {
                var roles = teamRoles.Where(tr => tr.TeamId == team.Id && tr.TeamMemberType == team.TeamMemberType)
                                     .Select(tr => tr.RoleId)
                                    .ToList();
                var permissionQuery = new PermissionsByRoleQuery(roles);
                await _eventBus.PublishAsync(permissionQuery);
                var rejectPermissions = teamPermissions.Where(tp => tp.TeamId == team.Id && tp.TeamMemberType == team.TeamMemberType && tp.Effect == false)
                                       .Select(tp => tp.PermissionId);
                var permissions = permissionQuery.Result.Union(
                        teamPermissions.Where(tp => tp.TeamId == team.Id && tp.TeamMemberType == team.TeamMemberType && tp.Effect)
                                       .Select(tp => tp.PermissionId)
                    ).Where(permission => rejectPermissions.All(rp => rp != permission));
                permissionIds.AddRange(permissions);
            }
            return permissionIds;
        }
    }

    [EventHandler]
    public async Task GetPermissionsByTeamWithUserAsync(PermissionsByTeamWithUserQuery query)
    {
        var teamQuery = new TeamByUserQuery(query.Dto.User);
        await _eventBus.PublishAsync(teamQuery);
        if (query.Dto.Teams.Count > 0)
        {
            teamQuery.Result = teamQuery.Result.Where(team => query.Dto.Teams.Contains(team.Id))
                                           .ToList();
        }
        var permissionByTeamQuery = new PermissionsByTeamQuery(teamQuery.Result);
        await _eventBus.PublishAsync(permissionByTeamQuery);
        query.Result.AddRange(permissionByTeamQuery.Result);
    }

    //[EventHandler(1)]
    //public async Task GetPermissionsByUserAsync(PermissionsByUserQuery query)
    //{
    //    var cacheUserModel = await _cacheClient.GetAsync<UserModel>(CacheKey.UserKey(query.User));
    //    if (cacheUserModel != null)
    //    {
    //        query.UserPermissionIds = cacheUserModel.Permissions.Select(tp =>
    //            KeyValuePair.Create(tp.PermissionId, value: tp.Effect)).ToList();
    //        return;
    //    }

    //    query.UserPermissionIds = await _authDbContext.Set<UserPermission>().AsNoTracking()
    //                                              .Where(up => up.UserId == query.User)
    //                                              .Select(tp => KeyValuePair.Create(
    //                                                  tp.PermissionId,
    //                                                  tp.Effect
    //                                              )).ToListAsync();
    //}

    [EventHandler(2)]
    public async Task GetPermissionsByUserTeamAsync(PermissionsByUserQuery query)
    {
        var teamQuery = new TeamByUserQuery(query.User);
        await _eventBus.PublishAsync(teamQuery);
        if (query.Teams is not null)
        {
            teamQuery.Result = teamQuery.Result.Where(team => query.Teams.Contains(team.Id))
                                               .ToList();
        }
        var permissionByTeamQuery = new PermissionsByTeamQuery(teamQuery.Result);
        await _eventBus.PublishAsync(permissionByTeamQuery);

        query.TeamPermissionIds = permissionByTeamQuery.Result;
    }

    [EventHandler(3)]
    public async Task GetPermissionsByUserRoleAsync(PermissionsByUserQuery query)
    {
        List<Guid> roles = new();
        var cacheUserModel = await _cacheClient.GetAsync<UserModel>(CacheKey.UserKey(query.User));
        if (cacheUserModel != null)
        {
            roles = cacheUserModel!.Roles.Select(ur => ur.Id).ToList();
        }
        if (!roles.Any())
        {
            roles = await _authDbContext.Set<UserRole>().AsNoTracking()
                                        .Where(ur => ur.UserId == query.User)
                                        .Select(ur => ur.RoleId)
                                        .ToListAsync();
        }

        var permissionByRoleQuery = new PermissionsByRoleQuery(roles);
        await _eventBus.PublishAsync(permissionByRoleQuery);

        query.RolePermissionIds = permissionByRoleQuery.Result;
    }

    [EventHandler(4)]
    public async Task GroupPermissionsByUserAsync(PermissionsByUserQuery query)
    {
        var permissionIds = query.UserPermissionIds.Where(kv => kv.Value)
                                    .Select(kv => kv.Key)
                                    .Union(query.TeamPermissionIds)
                                    .Union(query.RolePermissionIds)
                                    .Except(query.UserPermissionIds.Where(kv => !kv.Value)
                                    .Select(kv => kv.Key))
                                    .ToList();

        //skip the first level menu without submenus
        Dictionary<Guid, Guid> itemSubMenuIds;
        var cachePermissions = await _cacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (cachePermissions?.Count > 0)
        {
            itemSubMenuIds = cachePermissions!.Where(p => permissionIds.Contains(p.ParentId) && p.Type == PermissionTypes.Menu && p.Enabled)
                .ToDictionary(p => p.Id, p => p.ParentId);
        }
        else
        {
            itemSubMenuIds = (await _permissionRepository.GetListAsync(p => permissionIds.Contains(p.GetParentId()) && p.Type == PermissionTypes.Menu && p.Enabled))
                 .ToDictionary(p => p.Id, p => p.GetParentId());
        }
        permissionIds.RemoveAll(id =>
        {
            var currentSubMenusIds = itemSubMenuIds.Where(p => p.Value == id)
                        .Select(e => e.Key)
                        .ToList();
            return currentSubMenusIds.Count > 0 && !currentSubMenusIds.Intersect(permissionIds).Any();
        });
        List<Guid> relationPermissionIds = new();
        List<Guid> cacheMissIds = new();

        foreach (var permissionId in permissionIds)
        {
            var cachePermission = cachePermissions?.FirstOrDefault(ap => ap.Id == permissionId);
            if (cachePermission == null)
            {
                _logger.LogDebug("Permission Cache Miss:{0}", permissionId);
                cacheMissIds.Add(permissionId);
            }
            else
            {
                relationPermissionIds.AddRange(cachePermission.ApiPermissions);
            }
        }

        if (cacheMissIds.Any())
        {
            relationPermissionIds.AddRange(_authDbContext.Set<PermissionRelation>().AsNoTracking()
                .Where(pr => cacheMissIds.Contains(pr.LeadingPermissionId))
                .Select(pr => pr.AffiliationPermissionId).ToList());
        }

        query.Result = permissionIds.Union(relationPermissionIds).ToList();
    }

    [EventHandler]
    public async Task GetUserApiPermissionCodeQueryAsync(UserApiPermissionCodeQuery userApiPermissionCodeQuery)
    {
        var userId = userApiPermissionCodeQuery.UserId;
        var appId = userApiPermissionCodeQuery.AppId;
        var cacheKey = CacheKey.UserApiPermissionCodeKey(userId, appId);

        userApiPermissionCodeQuery.Result = (await _cacheClient.GetOrSetAsync(cacheKey, () =>
        {
            var userPermissionIds = _userDomainService.GetPermissionIdsAsync(userId).Result;
            return new CacheEntry<List<string>>(_permissionRepository.GetPermissionCodes(p => p.AppId == appId
                                && p.Type == PermissionTypes.Api && userPermissionIds.Contains(p.Id) && p.Enabled))
            {
                SlidingExpiration = TimeSpan.FromSeconds(5)
            };
        }))!;
    }

    #endregion

    [EventHandler]
    public async Task CollectMenuListQueryAsync(FavoriteMenuListQuery favoriteMenuListQuery)
    {
        var permissionIds = await _cacheClient.GetAsync<HashSet<Guid>>(CacheKey.UserMenuCollectKey(favoriteMenuListQuery.UserId));
        if (permissionIds == null)
        {
            return;
        }
        favoriteMenuListQuery.Result = (await _permissionRepository.GetListAsync(p => permissionIds.Contains(p.Id)))
            .Select(p => new SelectItemDto<Guid>
            {
                Value = p.Id,
                Text = p.Name
            }).ToList();
    }
}
