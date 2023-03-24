// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class QueryHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly AuthDbContext _authDbContext;
    readonly UserDomainService _userDomainService;
    readonly IMultilevelCacheClient _multilevelCacheClient;
    readonly IEventBus _eventBus;

    public QueryHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        IMultilevelCacheClient multilevelCacheClient,
        IEventBus eventBus)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _multilevelCacheClient = multilevelCacheClient;
        _eventBus = eventBus;
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
            var (creator, modifier) = _multilevelCacheClient.GetActionInfoAsync(role.Creator, role.Modifier).Result;
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
        var (creator, modifier) = await _multilevelCacheClient.GetActionInfoAsync(role.Creator, role.Modifier);
        query.Result.Creator = creator;
        query.Result.Modifier = modifier;
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
            var parentRoles = new List<Guid>();
            parentRoles.Add(roleId);
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
        return all.Where(p => p.ParentId == parentId)
            .OrderBy(p => p.Order)
            .Select(p => new AppPermissionDto
            {
                AppId = p.AppId,
                PermissonId = p.Id,
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
    public async Task PerimissionDetailQueryAsync(MenuPermissionDetailQuery menuPermissionDetailQuery)
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
            Code = permission.Code,
            Url = permission.Url,
            Type = permission.Type,
            Id = permission.Id,
            Enabled = permission.Enabled,
            ParentId = permission.ParentId,
            AppId = permission.AppId,
            Order = permission.Order,
            ApiPermissions = permission.ChildPermissionRelations.Select(pr => pr.ChildPermissionId).ToList(),
            Roles = permission.RolePermissions.Where(rp => rp.Effect).Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name, rp.Role.Code, rp.Role.Limit, rp.Role.AvailableQuantity)).ToList(),
            Teams = permission.TeamPermissions.Where(rp => rp.Effect).Select(tp => new TeamSelectDto(tp.Team.Id, tp.Team.Name, tp.Team.Avatar.Url)).ToList(),
            Users = permission.UserPermissions.Where(rp => rp.Effect).Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.DisplayName,
                Avatar = up.User.Avatar
            }).ToList()
        };
    }

    [EventHandler]
    public async Task PerimissionDetailQueryAsync(ApiPermissionDetailQuery apiPermissionDetailQuery)
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

        var menus = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        if (menus == null || menus.Count < 1)
        {
            menus = (await _permissionRepository.GetListAsync(p => p.AppId == appMenuListQuery.AppId
                                && p.Type == PermissionTypes.Menu && userPermissionIds.Contains(p.Id) && p.Enabled)).Adapt<List<CachePermission>>();
        }
        menus = menus.Where(p => p.AppId == appMenuListQuery.AppId && p.Type == PermissionTypes.Menu && userPermissionIds.Contains(p.Id) && p.Enabled).ToList();

        appMenuListQuery.Result = GetMenus(menus, Guid.Empty);

        List<MenuDto> GetMenus(List<CachePermission> menus, Guid parentId)
        {
            return menus.Where(m => m.ParentId == parentId)
                .OrderBy(m => m.Order)
                .Select(m => new MenuDto
                {
                    Id = m.Id,
                    Name = m.Name,
                    Code = m.Code,
                    Url = m.Url,
                    Icon = m.Icon,
                    Children = GetMenus(menus, m.Id)
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
    public async Task AppPermissionAuthorizedQueryAsync(AppPermissionAuthorizedQuery appPermissionAuthorizedQuery)
    {
        appPermissionAuthorizedQuery.Result = await _userDomainService.AuthorizedAsync(appPermissionAuthorizedQuery.AppId,
                    appPermissionAuthorizedQuery.Code, appPermissionAuthorizedQuery.UserId);
    }

    [EventHandler]
    public async Task GetPermissionsByRoleAsync(PermissionsByRoleQuery query)
    {
        if (query.Roles.Count == 0) return;
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
            var rolePermissons = await _authDbContext.Set<Role>()
                                          .Where(r => r.Enabled == true && roleIds.Contains(r.Id))
                                          .Include(r => r.ChildrenRoles)
                                          .Include(r => r.Permissions)
                                          .Select(r => new { r.Permissions, r.ChildrenRoles })
                                          .ToListAsync();
            foreach (var rolePermisson in rolePermissons)
            {
                var childPermissions = new List<Guid>();
                if (rolePermisson.ChildrenRoles.Any())
                {
                    childPermissions = await GetPermissionsAsync(rolePermisson.ChildrenRoles.Select(cr => cr.RoleId));
                    var rejectPermisisons = rolePermisson.Permissions.Where(p => p.Effect is false).ToList();
                    childPermissions = childPermissions.Where(p => rejectPermisisons.All(rp => rp.PermissionId != p)).ToList();
                    childPermissions.AddRange(rolePermisson.Permissions.Where(p => p.Effect is true).Select(p => p.PermissionId));
                    permissions.AddRange(childPermissions);
                }
                childPermissions.AddRange(rolePermisson.Permissions.Where(p => p.Effect is true).Select(p => p.PermissionId));
                permissions.AddRange(childPermissions);
            }

            return permissions.Distinct().ToList();
        }

        async Task<List<Guid>> GetPermissionsByCacheAsync(IEnumerable<Guid> roleIds)
        {
            roleIds = roleIds.Distinct().ToList();
            var permissions = new List<Guid>();
            var roleCacheKeys = roleIds.Select(e => CacheKey.RoleKey(e));
            var rolePermissons = await _multilevelCacheClient.GetListAsync<CacheRole>(roleCacheKeys);
            foreach (var rolePermisson in rolePermissons)
            {
                if (rolePermisson == null)
                {
                    continue;
                }
                var childPermissions = new List<Guid>();
                if (rolePermisson.ChildrenRoles != null && rolePermisson.ChildrenRoles.Any())
                {
                    childPermissions = await GetPermissionsByCacheAsync(rolePermisson.ChildrenRoles);
                    var rejectPermisisons = rolePermisson.Permissions.Where(p => p.Effect is false).ToList();
                    childPermissions = childPermissions.Where(p => rejectPermisisons.All(rp => rp.PermissionId != p)).ToList();
                    childPermissions.AddRange(rolePermisson.Permissions.Where(p => p.Effect is true).Select(p => p.PermissionId));
                    permissions.AddRange(childPermissions);
                }
                childPermissions.AddRange(rolePermisson.Permissions.Where(p => p.Effect is true).Select(p => p.PermissionId));
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

        var permissionIds = await GetPermissionsByCacheAsync(teamIds);
        if (permissionIds == null || permissionIds.Count < 1)
        {
            permissionIds = await GetPermissionsAsync(teamIds);
        }

        query.Result.AddRange(permissionIds);

        async Task<List<Guid>> GetPermissionsAsync(IEnumerable<Guid> teamIds)
        {
            var permissionIds = new List<Guid>();
            var teamRoles = await _authDbContext.Set<TeamRole>()
                                           .Where(tr => teamIds.Contains(tr.TeamId))
                                           .ToListAsync();
            var teamPermissions = await _authDbContext.Set<TeamPermission>()
                                                     .Where(tp => teamIds.Contains(tp.TeamId))
                                                     .ToListAsync();
            foreach (var team in query.Teams)
            {
                var roles = teamRoles.Where(tr => tr.TeamId == team.Id && tr.TeamMemberType == team.TeamMemberType)
                                     .Select(tr => tr.RoleId)
                                    .ToList();
                var permissionQuery = new PermissionsByRoleQuery(roles);
                await _eventBus.PublishAsync(permissionQuery);
                var rejectPermisisons = teamPermissions.Where(tp => tp.TeamId == team.Id && tp.TeamMemberType == team.TeamMemberType && tp.Effect == false)
                                       .Select(tp => tp.PermissionId);
                var permissions = permissionQuery.Result.Union(
                        teamPermissions.Where(tp => tp.TeamId == team.Id && tp.TeamMemberType == team.TeamMemberType && tp.Effect == true)
                                       .Select(tp => tp.PermissionId)
                    ).Where(permission => rejectPermisisons.All(rp => rp != permission));
                permissionIds.AddRange(permissions);
            }
            return permissionIds;
        }

        async Task<List<Guid>> GetPermissionsByCacheAsync(IEnumerable<Guid> teamIds)
        {
            var permissionIds = new List<Guid>();

            var teamIdCacheKeys = teamIds.Select(e => CacheKey.TeamKey(e));
            var cacheTeams = await _multilevelCacheClient.GetListAsync<TeamDetailDto>(teamIdCacheKeys);
            if (cacheTeams != null)
            {
                foreach (var team in cacheTeams)
                {
                    if (team == null)
                    {
                        continue;
                    }
                    var roleIds = team!.TeamAdmin.Roles.Union(team.TeamMember.Roles).ToList();
                    var permissionQuery = new PermissionsByRoleQuery(roleIds);
                    await _eventBus.PublishAsync(permissionQuery);

                    var permissionRelations = team.TeamMember.Permissions;
                    permissionRelations.AddRange(team.TeamAdmin.Permissions);

                    var rejectPermisisons = permissionRelations.Where(e => e.Effect == false).Select(tp => tp.PermissionId);
                    var permissions = permissionQuery.Result.Union(permissionRelations.Where(tp => tp.Effect == true)
                                           .Select(tp => tp.PermissionId)).Where(permission => rejectPermisisons.All(rp => rp != permission));
                    permissionIds.AddRange(permissions);
                }
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

    [EventHandler]
    public async Task GetPermissionsByUserAsync(PermissionsByUserQuery query)
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

        var cachePermissions = await _multilevelCacheClient.GetAsync<List<CachePermission>>(CacheKey.AllPermissionKey());
        var permissionIds = await GetPermissionsByCacheAsync(query.User, permissionByTeamQuery.Result);
        if (permissionIds == null || permissionIds.Count() < 1)
        {
            permissionIds = await GetPermissionsAsync(query.User, permissionByTeamQuery.Result);
        }
        query.Result.AddRange(permissionIds);

        //Filter out empty menus that do not have submenu permissions.
        await FilterEmptyMenus();

        async Task<List<Guid>> GetPermissionsAsync(Guid userId, List<Guid> teamPermissionIds)
        {
            var roles = await _authDbContext.Set<UserRole>().AsNoTracking()
                                            .Where(ur => ur.UserId == query.User)
                                            .Select(ur => ur.RoleId)
                                            .ToListAsync();
            var permissions = await _authDbContext.Set<UserPermission>().AsNoTracking()
                                                  .Where(up => up.UserId == query.User)
                                                  .ToListAsync();
            var rejectPermisisons = permissions.Where(permission => permission.Effect == false)
                                               .Select(tp => tp.PermissionId);
            var permissionByRoleQuery = new PermissionsByRoleQuery(roles);
            await _eventBus.PublishAsync(permissionByRoleQuery);
            var userPermission = permissionByRoleQuery.Result
                                    .Union(permissions.Select(permission => permission.PermissionId))
                                    .Union(teamPermissionIds)
                                    .Where(permission => rejectPermisisons.All(rp => rp != permission));
            var apiPermissions = await _authDbContext.Set<PermissionRelation>().AsNoTracking()
                    .Where(pr => userPermission.Contains(pr.ParentPermissionId))
                    .Select(pr => pr.ChildPermissionId).ToListAsync();
            return userPermission.Union(apiPermissions).ToList();
        }

        async Task<List<Guid>> GetPermissionsByCacheAsync(Guid userId, List<Guid> teamPermissionIds)
        {
            var cacheUserModel = await _multilevelCacheClient.GetAsync<UserModel>(CacheKeyConsts.UserKey(query.User));
            if (cacheUserModel == null || cachePermissions == null)
            {
                return new List<Guid>();
            }

            var roles = cacheUserModel!.Roles.Select(ur => ur.Id).ToList();
            var permissions = cacheUserModel.Permissions;
            var rejectPermisisons = permissions.Where(permission => permission.Effect == false)
                                               .Select(tp => tp.PermissionId);
            var permissionByRoleQuery = new PermissionsByRoleQuery(roles);
            await _eventBus.PublishAsync(permissionByRoleQuery);
            var userPermission = permissionByRoleQuery.Result
                                    .Union(permissions.Select(permission => permission.PermissionId))
                                    .Union(teamPermissionIds)
                                    .Where(permission => rejectPermisisons.All(rp => rp != permission));

            var apiPermissions = new List<Guid>();
            foreach (var idItem in userPermission)
            {
                if (cachePermissions!.Any(e => e.Id == idItem))
                {
                    apiPermissions.AddRange(cachePermissions!.First(e => e.Id == idItem).ApiPermissions);
                }
            }
            return userPermission.Union(apiPermissions).ToList();
        }

        async Task FilterEmptyMenus()
        {
            permissionIds = new List<Guid>();
            if (cachePermissions != null && cachePermissions.Count > 0)
            {
                var cacheSubMenus = cachePermissions!.Where(p => query.Result.Contains(p.ParentId) && p.Type == PermissionTypes.Menu && p.Enabled).ToList();
                foreach (var item in query.Result)
                {
                    var itemSubMenuIds = cacheSubMenus.Where(p => p.ParentId == item).Select(e => e.Id).ToList();
                    if (itemSubMenuIds.Count > 0 && itemSubMenuIds.Intersect(query.Result).Count() < 1)
                    {
                        continue;
                    }
                    permissionIds!.Add(item);
                }
            }
            else
            {
                var subMenus = await _permissionRepository.GetListAsync(p => query.Result.Contains(p.ParentId) && p.Type == PermissionTypes.Menu && p.Enabled);
                foreach (var item in query.Result)
                {
                    var itemSubMenuIds = subMenus.Where(p => p.ParentId == item).Select(e => e.Id).ToList();
                    if (itemSubMenuIds.Count > 0 && itemSubMenuIds.Intersect(query.Result).Count() < 1)
                    {
                        continue;
                    }
                    permissionIds!.Add(item);
                }
            }
            query.Result = permissionIds!;
        }
    }

    [EventHandler]
    public async Task GetUserApiPermissionCodeQueryAsync(UserApiPermissionCodeQuery userApiPermissionCodeQuery)
    {
        var userId = userApiPermissionCodeQuery.UserId;
        var appId = userApiPermissionCodeQuery.AppId;
        var cacheKey = CacheKey.UserApiPermissionCodeKey(userId, appId);

        userApiPermissionCodeQuery.Result = (await _multilevelCacheClient.GetOrSetAsync(cacheKey, new CombinedCacheEntry<List<string>>
        {
            DistributedCacheEntryFunc = () =>
            {
                var userPermissionIds = _userDomainService.GetPermissionIdsAsync(userId).Result;
                return new CacheEntry<List<string>>(_permissionRepository.GetPermissionCodes(p => p.AppId == appId
                                    && p.Type == PermissionTypes.Api && userPermissionIds.Contains(p.Id) && p.Enabled))
                {
                    SlidingExpiration = TimeSpan.FromSeconds(5)
                };
            },
            MemoryCacheEntryOptionsAction = (memoryCacheOptions) =>
            {
                memoryCacheOptions.AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5);
            }
        }))!;
    }

    #endregion

    [EventHandler]
    public async Task CollectMenuListQueryAsync(FavoriteMenuListQuery favoriteMenuListQuery)
    {
        var permissionIds = await _multilevelCacheClient.GetAsync<HashSet<Guid>>(CacheKey.UserMenuCollectKey(favoriteMenuListQuery.UserId));
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
