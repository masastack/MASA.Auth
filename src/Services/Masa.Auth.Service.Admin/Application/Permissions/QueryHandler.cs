// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
            role.Users.Select(ur => new UserSelectDto(ur.User.Id, ur.User.Name, ur.User.Account, ur.User.PhoneNumber, ur.User.Email, ur.User.Avatar)).ToList(),
            role.Teams.Select(tr => new TeamSelectDto(tr.Team.Id, tr.Team.Name, tr.Team.Avatar.Url)).ToList()
        );
    }

    [EventHandler]
    public async Task GetTopRoleSelectAsync(TopRoleSelectQuery query)
    {
        var roleSelect = await _authDbContext.Set<RoleRelation>()
                                    .Include(r => r.ParentRole)
                                    .Where(r => r.RoleId == query.RoleId)
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

    private async Task<List<RoleSelectDto>> GetRoleSelectAsync()
    {
        var roleSelect = await _authDbContext.Set<Role>()
                                        .Select(r => new RoleSelectDto(r.Id, r.Name, r.Limit, r.AvailableQuantity))
                                        .ToListAsync();

        return roleSelect;
    }

    [EventHandler]
    public async Task GetPermissionsByRoleAsync(PermissionsByRoleQuery query)
    {
        query.Result = await GetPermissions(query.Roles);

        async Task<List<Guid>> GetPermissions(List<Guid> roleIds)
        {
            var permissions = new List<Guid>();
            var roles = await _authDbContext.Set<Role>()
                                          .Include(r => r.ChildrenRoles)
                                          .Include(r => r.Permissions)
                                          .Where(r => roleIds.Contains(r.Id))
                                          .Select(r => new { r.Permissions, r.ChildrenRoles })
                                          .ToListAsync();
            permissions.AddRange(roles.SelectMany(r => r.Permissions.Select(p => p.PermissionId)));
            var childRoles = roles.SelectMany(r => r.ChildrenRoles.Select(cr => cr.RoleId)).ToList();
            if (childRoles.Count > 0)
            {
                permissions.AddRange(await GetPermissions(childRoles));
            }

            return permissions;
        }
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
    public async Task ChildMenuPermissionsQueryAsync(ChildMenuPermissionsQuery childMenuPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.ParentId == childMenuPermissionsQuery.PermissionId
                            && p.Type != PermissionTypes.Api);
        childMenuPermissionsQuery.Result = permissions
            .Select(p => new PermissionDto
            {
                Id = p.Id,
                Name = p.Name,
                Type = p.Type
            }).ToList();
    }

    [EventHandler]
    public async Task ApplicationPermissionsQueryAsync(ApplicationPermissionsQuery applicationPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == applicationPermissionsQuery.SystemId);

        applicationPermissionsQuery.Result = permissions.Select(p => new AppPermissionDto
        {
            AppId = p.AppId,
            PermissonId = p.Id,
            PermissonName = p.Name,
            Type = p.Type
        }).ToList();
    }

    [EventHandler]
    public async Task ApiPermissionSelectQueryAsync(ApiPermissionSelectQuery apiPermissionSelectQuery)
    {
        Expression<Func<Permission, bool>> condition = permission => permission.Type == PermissionTypes.Api;
        if (!string.IsNullOrEmpty(apiPermissionSelectQuery.Name))
            condition = condition.And(permission => permission.Name.Contains(apiPermissionSelectQuery.Name));

        var permissions = await _permissionRepository.GetPaginatedListAsync(condition, 0, apiPermissionSelectQuery.MaxCount);
        apiPermissionSelectQuery.Result = permissions.Select(p => new SelectItemDto<Guid>
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
            AppId = permission.AppId
        };
    }

    #endregion
}
