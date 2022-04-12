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
    private async Task GetRolePaginationAsync(RolePaginationQuery query)
    {
        Expression<Func<Role, bool>> condition = role => role.Enabled == query.Enabled;
        if (!string.IsNullOrEmpty(query.Search))
            condition = condition.And(user => user.Name.Contains(query.Search));

        var roles = await _roleRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(User.ModificationTime)] = true,
                [nameof(User.CreationTime)] = true,
            }
        });

        //query.Result = new(roles.Total, roles.TotalPages, roles.Result.Select(u => new RoleDto
        //{

        //}));
    }

    [EventHandler]
    private async Task GetRoleDetailAsync(RoleDetailQuery query)
    {
        var role = await _roleRepository.FindAsync(u => u.Id == query.RoleId);
        if (role is null) throw new UserFriendlyException("This role data does not exist");

        //query.Result = new(role.Name, role.Description, role.Enabled, role.RolePermissions.Select(rp => rp.Id).ToList(), role.RoleItems.Select(ri => ri.Role.Id).ToList());
    }

    [EventHandler]
    private async Task GetRoleSelectAsync(RoleSelectQuery query)
    {
        query.Result = await _authDbContext.Set<Role>().Select(r => new RoleSelectDto(r.Id, r.Name)).ToListAsync();
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
            Roles = permission.RolePermissions.Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name)).ToList(),
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
