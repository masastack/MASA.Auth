namespace Masa.Auth.Service.Admin.Application.Permissions;

public class QueryHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly AuthDbContext _authDbContext;

    public QueryHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository, AuthDbContext authDbContext)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    public async Task GetRoleListAsync(GetRolesQuery query)
    {
        Expression<Func<Role, bool>> condition = role => role.Hidden == false;
        if (query.Enabled is not null)
            condition = condition.And(role => role.Enabled == query.Enabled);
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(user => user.Name.Contains(query.Search));

        var roles = await _roleRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Role.ModificationTime)] = true,
                [nameof(Role.CreationTime)] = true,
            }
        });

        query.Result = new(roles.Total, roles.Result.Select(r => new RoleDto(r.Id, r.Name, r.Limit, r.Description, r.Enabled, r.CreationTime, r.ModificationTime, r.CreatorUser?.Name ?? "", r.ModifierUser?.Name ?? "")).ToList());
    }

    [EventHandler]
    public async Task GetRoleDetailAsync(RoleDetailQuery query)
    {
        var role = await _roleRepository.GetDetailAsync(query.RoleId);
        if (role is null) throw new UserFriendlyException("This role data does not exist");

        query.Result = new(role.Id, role.Name, role.Description, role.Enabled, role.Limit, role.Permissions.Select(rp => rp.Id).ToList(), role.ChildrenRoles.Select(ri => ri.Role.Id).ToList(), role.Users.Select(u => u.Id).ToList(), role.CreationTime, role.ModificationTime, role.CreatorUser?.Name ?? "", role.ModifierUser?.Name ?? "", role.QuantityAvailable);
    }

    [EventHandler]
    public async Task GetTopRoleSelectAsync(TopRoleSelectQuery query)
    {
        var roleSelect = await _authDbContext.Set<RoleRelation>()
                                    .Include(r => r.ParentRole)
                                    .Where(r => r.RoleId == query.RoleId)
                                    .Select(r => new RoleSelectDto(r.ParentRole.Id, r.ParentRole.Name, r.ParentRole.QuantityAvailable))
                                    .ToListAsync();

        query.Result = roleSelect;
    }

    [EventHandler]
    public async Task GetRoleSelectForUserAsync(RoleSelectForUserQuery query)
    {
        var roleSelect = new List<RoleSelectDto>();
        if (query.UserId != Guid.Empty)
        {
            var bindRoles = await _authDbContext.Set<UserRole>()
                                    .Where(ur => ur.UserId == query.UserId)
                                    .Select(ur => ur.RoleId)
                                    .ToListAsync();
            roleSelect = await GetRoleSelectAsync(bindRoles, 1);
        }
        else
        {
            roleSelect = await GetRoleSelectAsync(new(), 0);
        }

        query.Result = roleSelect;
    }

    [EventHandler]
    public async Task GetRoleSelectForRoleAsync(RoleSelectForRoleQuery query)
    {
        var roleSelect = new List<RoleSelectDto>();
        if (query.RoleId != Guid.Empty)
        {
            var roleRelations = await _authDbContext.Set<RoleRelation>().ToListAsync();
            var bindRoles = roleRelations.Where(r => r.ParentId == query.RoleId)
                                         .Select(r => r.RoleId)
                                         .ToList();
            roleSelect = await GetRoleSelectAsync(bindRoles, 0);
            var parentRoles = FindParentRoles(roleRelations, query.RoleId);
            roleSelect = roleSelect.Where(r => parentRoles.Contains(r.Id) is false).ToList();
        }
        else
        {
            roleSelect = await GetRoleSelectAsync(new(), 0);
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

    public async Task GetRoleSelectForTeamAsync(RoleSelectForTeamQuery query)
    {
        await Task.CompletedTask;
    }

    private async Task<List<RoleSelectDto>> GetRoleSelectAsync(List<Guid> bindRoles, int limit)
    {
        var roleSelect = await _authDbContext.Set<Role>()
                                        .Where(r => r.Hidden == false && (bindRoles.Contains(r.Id) || r.Limit == 0 || r.QuantityAvailable >= limit))
                                        .Select(r => new RoleSelectDto(r.Id, r.Name, r.QuantityAvailable))
                                        .ToListAsync();

        return roleSelect;
    }

    #region Permission

    [EventHandler]
    public void PermissionTypesQueryAsync(PermissionTypesQuery permissionTypesQuery)
    {
        permissionTypesQuery.Result = Enum<PermissionTypes>.GetDescriptionList().Select(pt => new SelectItemDto<int>
        {
            Text = pt.desc,
            Value = (int)pt.value
        }).ToList();
    }

    [EventHandler]
    public async Task ApiPermissionsQueryAsync(ApiPermissionListQuery apiPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == apiPermissionsQuery.SystemId
                            && p.Type == PermissionTypes.Api);
        apiPermissionsQuery.Result = permissions.GroupBy(p => p.AppId)
            .Select(pg => new AppPermissionDto
            {
                AppId = pg.Key,
                Permissions = pg.Select(p => new PermissionDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                }).ToList()
            }).ToList();
    }

    [EventHandler]
    public async Task MenuPermissionsQueryAsync(MenuPermissionListQuery menuPerimissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == menuPerimissionsQuery.SystemId
                            && p.Type != PermissionTypes.Api);

        menuPerimissionsQuery.Result = permissions.GroupBy(p => p.AppId)
            .Select(pg => new AppPermissionDto
            {
                AppId = pg.Key,
                Permissions = GetPermissionChild(Guid.Empty, pg.ToList())
            }).ToList();
    }

    [EventHandler]
    public async Task ApiPermissionSelectQueryAsync(ApiPermissionSelectQuery apiPermissionSelectQuery)
    {
        Expression<Func<Permission, bool>> condition = permission => permission.Type == PermissionTypes.Api;
        if (!string.IsNullOrEmpty(apiPermissionSelectQuery.Name))
            condition = condition.And(permission => permission.Name.Contains(apiPermissionSelectQuery.Name));

        var permissions = await _permissionRepository.GetPaginatedListAsync(condition, 0, apiPermissionSelectQuery.MaxCount, null);
        apiPermissionSelectQuery.Result = permissions.Select(p => new SelectItemDto<Guid>
        {
            Value = p.Id,
            Text = p.Name
        }).ToList();
    }

    private List<PermissionDto> GetPermissionChild(Guid parentId, List<Permission> source)
    {
        return source.Where(p => p.ParentId == parentId).Select(p => new PermissionDto
        {
            Id = p.Id,
            Name = p.Name,
            Code = p.Code,
            Children = GetPermissionChild(p.Id, source)
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
            ApiPermissions = permission.Permissions.Select(pr => new PermissionDto
            {
                Id = pr.Id,
                Name = pr.ChildPermission.Name,
                Code = pr.ChildPermission.Code
            }).ToList(),
            Roles = permission.RolePermissions.Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name, rp.Role.QuantityAvailable)).ToList(),
            Teams = permission.TeamPermissions.Select(tp => new TeamSelectDto(tp.Team.Id, tp.Team.Name, tp.Team.Avatar.Url)).ToList(),
            Users = permission.UserPermissions.Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.Name,
                Avatar = up.User.Avatar
            }).ToList(),
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
            Id = permission.Id
        };
    }

    #endregion
}
