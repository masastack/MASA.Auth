﻿namespace Masa.Auth.Service.Admin.Application.Permissions;

public class QueryHandler
{
    readonly IPermissionRepository _permissionRepository;

    public QueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    [EventHandler]
    public async Task ApiPermissionsQueryAsync(ApiPermissionListQuery apiPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == apiPermissionsQuery.SystemId
                            && p.Type == PermissionType.Api);
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
                            && p.Type != PermissionType.Api);

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
            RoleItems = permission.RolePermissions.Select(rp => new RoleSelectDto
            {
                Id = rp.Role.Id,
                Name = rp.Role.Name,
                Code = rp.Role.Code
            }).ToList(),
            TeamItems = permission.TeamPermissions.Select(tp => new TeamSelectDto
            {
                Id = tp.Team.Id,
                Name = tp.Team.Name,
                Avatar = tp.Team.Avatar.Url
            }).ToList(),
            UserItems = permission.UserPermissions.Select(up => new UserSelectDto
            {
                Id = up.User.Id,
                Name = up.User.Name,
                Avatar = up.User.Avatar
            }).ToList(),
        };
    }
}
