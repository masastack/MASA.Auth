// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class QueryHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly AuthDbContext _authDbContext;
    readonly UserDomainService _userDomainService;
    readonly IMemoryCacheClient _memoryCacheClient;
    readonly IEventBus _eventBus;
    CallGroup _callGroup;

    public QueryHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        AuthDbContext authDbContext,
        UserDomainService userDomainService,
        IMemoryCacheClient memoryCacheClient,
        IEventBus eventBus,
        CallGroup callGroup)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
        _userDomainService = userDomainService;
        _memoryCacheClient = memoryCacheClient;
        _eventBus = eventBus;
        _callGroup = callGroup;
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
        var roles = await roleQuery.Include(s => s.CreateUser)
                                   .Include(s => s.ModifyUser)
                                   .OrderByDescending(s => s.ModificationTime)
                                   .ThenByDescending(s => s.CreationTime)
                                   .Skip((query.Page - 1) * query.PageSize)
                                   .Take(query.PageSize)
                                   .ToListAsync();

        query.Result = new(total, roles.Select(role => (RoleDto)role).ToList());
    }

    [EventHandler]
    public async Task GetRoleDetailAsync(RoleDetailQuery query)
    {
        var role = await _roleRepository.GetDetailAsync(query.RoleId);
        if (role is null) throw new UserFriendlyException("This role data does not exist");

        query.Result = role;
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
        if (role is null) throw new UserFriendlyException("This role data does not exist");

        query.Result = new(
            role.Users.Select(ur => new UserSelectDto(ur.User.Id, ur.User.Name, ur.User.DisplayName, ur.User.Account, ur.User.PhoneNumber, ur.User.Email, ur.User.Avatar)).ToList(),
            role.Teams.Select(tr => new TeamSelectDto(tr.Team.Id, tr.Team.Name, tr.Team.Avatar.Url)).ToList()
        );
    }

    [EventHandler]
    public async Task GetTopRoleSelectAsync(TopRoleSelectQuery query)
    {
        var roleSelect = await _authDbContext.Set<RoleRelation>()
                                    .Include(r => r.ParentRole)
                                    .Where(r => r.ParentRole.Enabled == true && r.RoleId == query.RoleId)
                                    .Select(r => new RoleSelectDto(r.ParentRole.Id, r.ParentRole.Name, r.ParentRole.Limit, r.ParentRole.AvailableQuantity))
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
                                         .Where(r => r.Enabled == true && r.Name.Contains(query.Name))
                                         .Select(r => new RoleSelectDto(r.Id, r.Name, r.Limit, r.AvailableQuantity))
                                         .ToListAsync();
        query.Result = roleSelect;
    }

    private async Task<List<RoleSelectDto>> GetRoleSelectAsync()
    {
        var roleSelect = await _authDbContext.Set<Role>()
                                        .Where(r => r.Enabled == true)
                                        .Select(r => new RoleSelectDto(r.Id, r.Name, r.Limit, r.AvailableQuantity))
                                        .ToListAsync();
        return roleSelect;
    }

    #endregion

    #region Permission

    [EventHandler]
    public void PermissionTypesQueryAsync(PermissionTypesQuery permissionTypesQuery)
    {
        permissionTypesQuery.Result = Enum<PermissionTypes>.GetEnumObjectDictionary().Select(pt => new SelectItemDto<int>
        {
            Text = pt.Value,
            Value = pt.Key
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
        if (!string.IsNullOrEmpty(apiPermissionSelectQuery.SystemId))
        {
            condition = condition.And(permission => permission.SystemId == apiPermissionSelectQuery.SystemId);
        }

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
            throw new UserFriendlyException($"this permission by id={menuPermissionDetailQuery.PermissionId} is api permission");
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
            Roles = permission.RolePermissions.Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name, rp.Role.Limit, rp.Role.AvailableQuantity)).ToList(),
            Teams = permission.TeamPermissions.Select(tp => new TeamSelectDto(tp.Team.Id, tp.Team.Name, tp.Team.Avatar.Url)).ToList(),
            Users = permission.UserPermissions.Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.Name,
                Avatar = up.User.Avatar
            }).ToList()
        };
    }

    [EventHandler]
    public async Task PerimissionDetailQueryAsync(ApiPermissionDetailQuery apiPermissionDetailQuery)
    {
        var permission = await _permissionRepository.FindAsync(apiPermissionDetailQuery.PermissionId)
                ?? throw new UserFriendlyException($"the permission id={apiPermissionDetailQuery.PermissionId} not found");
        if (permission.Type != PermissionTypes.Api)
        {
            throw new UserFriendlyException($"this permission by id={apiPermissionDetailQuery.PermissionId} is not api permission");
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
        var menus = await _permissionRepository.GetListAsync(p => p.AppId == appMenuListQuery.AppId
                            && p.Type == PermissionTypes.Menu && userPermissionIds.Contains(p.Id) && p.Enabled);
        appMenuListQuery.Result = GetMenus(menus.ToList(), Guid.Empty);

        List<MenuDto> GetMenus(List<Permission> menus, Guid parentId)
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
        query.Result = await GetPermissions(query.Roles);

        async Task<List<Guid>> GetPermissions(IEnumerable<Guid> roleIds)
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
                    childPermissions = await GetPermissions(rolePermisson.ChildrenRoles.Select(cr => cr.RoleId));
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
            query.Result.AddRange(permissions);
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
        var roles = await _authDbContext.Set<UserRole>()
                                        .Where(ur => ur.UserId == query.User)
                                        .Select(ur => ur.RoleId)
                                        .ToListAsync();
        var permissions = await _authDbContext.Set<UserPermission>()
                                              .Where(up => up.UserId == query.User)
                                              .ToListAsync();
        var rejectPermisisons = permissions.Where(permission => permission.Effect == false)
                                           .Select(tp => tp.PermissionId);
        var permissionByRoleQuery = new PermissionsByRoleQuery(roles);
        await _eventBus.PublishAsync(permissionByRoleQuery);
        var userPermission = permissionByRoleQuery.Result.Union(
                permissions.Where(permission => permission.Effect is true)
                           .Select(permission => permission.PermissionId)
            ).Where(permission => rejectPermisisons.All(rp => rp != permission));
        query.Result.AddRange(userPermission);
        query.Result.AddRange(permissionByTeamQuery.Result);
    }

    [EventHandler]
    public async Task GetUserElementPermissionCodeQueryAsync(UserElementPermissionCodeQuery userElementPermissionCodeQuery)
    {
        var userId = userElementPermissionCodeQuery.UserId;
        var appId = userElementPermissionCodeQuery.AppId;
        var cacheKey = CacheKey.UserElementPermissionCodeKey(userId, appId);
        //todo use golang singleflight ideo replace lock fixed cache break
        var codeList = await _memoryCacheClient.GetAsync<List<string>>(cacheKey);
        if (codeList == null)
        {
            var userPermissionIds = await _userDomainService.GetPermissionIdsAsync(userId);
            codeList = _permissionRepository.GetPermissionCodes(p => p.AppId == appId
                                && p.Type == PermissionTypes.Element && userPermissionIds.Contains(p.Id) && p.Enabled);
            _memoryCacheClient.Set(cacheKey, codeList, new CombinedCacheEntryOptions<List<string>>
            {
                DistributedCacheEntryOptions = new Microsoft.Extensions.Caching.Distributed.DistributedCacheEntryOptions
                {
                    SlidingExpiration = TimeSpan.FromSeconds(5)
                },
                MemoryCacheEntryOptions = new Microsoft.Extensions.Caching.Memory.MemoryCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(5)
                }
            });
        };
        userElementPermissionCodeQuery.Result = codeList;
        //temporary allow all api route
        userElementPermissionCodeQuery.Result.Add("*");
    }

    #endregion

    [EventHandler]
    public async Task CollectMenuListQueryAsync(FavoriteMenuListQuery favoriteMenuListQuery)
    {
        var permissionIds = await _memoryCacheClient.GetAsync<HashSet<Guid>>(CacheKey.UserMenuCollectKey(favoriteMenuListQuery.UserId));
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
