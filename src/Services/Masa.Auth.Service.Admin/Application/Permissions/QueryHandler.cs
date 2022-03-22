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
    public async Task FuncPermissionsQueryAsync(FuncPermissionListQuery funcPerimissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == funcPerimissionsQuery.SystemId
                            && p.Type != PermissionTypes.Api);

        funcPerimissionsQuery.Result = permissions.GroupBy(p => p.AppId)
                .Select(pg => new AppPermissionDto
                {
                    AppId = pg.Key,
                    Permissions = GetPermissionChild(Guid.Empty, pg.ToList())
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
    public async Task PerimissionDetailQueryAsync(PermissionDetailQuery perimissionDetailQuery)
    {
        var permission = await _permissionRepository.GetByIdAsync(perimissionDetailQuery.PermissionId);
        perimissionDetailQuery.Result = new PermissionDetailDto
        {
            Name = permission.Name,
            Description = permission.Description,
            Icon = permission.Icon,
            Code = permission.Code,
            Url = permission.Url,
            Type = permission.Type,
            Id = permission.Id,
            Enabled = permission.Enabled,
            Roles = permission.RolePermissions.Select(rp => new RoleSelectDto(rp.Role.Id, rp.Role.Name)).ToList(),
            Teams = permission.TeamPermissions.Select(tp => new TeamSelectDto
            {
                Id = tp.Team.Id,
                Name = tp.Team.Name,
                Avatar = tp.Team.Avatar.Url
            }).ToList(),
            Users = permission.UserPermissions.Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.Name,
                Avatar = up.User.Avatar
            }).ToList(),
        };
    }
}
