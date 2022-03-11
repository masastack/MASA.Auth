namespace Masa.Auth.Service.Application.Permissions;

public class QueryHandler
{
    readonly IPermissionRepository _permissionRepository;

    public QueryHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    [EventHandler]
    public async Task ApiPermissionsQueryAsync(ApiPermissionsQuery apiPermissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == apiPermissionsQuery.SystemId
                            && p.Type == PermissionType.Api);
        apiPermissionsQuery.Result = permissions.GroupBy(p => p.AppId)
            .Select(pg => new AppPermissionItem
            {
                AppId = pg.Key,
                Permissions = pg.Select(p => new PermissionItem
                {
                    Id = p.Id,
                    Name = p.Name,
                    Code = p.Code,
                }).ToList()
            }).ToList();
    }

    [EventHandler]
    public async Task FuncPermissionsQueryAsync(FuncPermissionsQuery funcPerimissionsQuery)
    {
        var permissions = await _permissionRepository.GetListAsync(p => p.SystemId == funcPerimissionsQuery.SystemId
                            && p.Type != PermissionType.Api);

        funcPerimissionsQuery.Result = permissions.GroupBy(p => p.AppId)
                .Select(pg => new AppPermissionItem
                {
                    AppId = pg.Key,
                    Permissions = GetPermissionChild(Guid.Empty, pg.ToList())
                }).ToList();
    }

    private List<PermissionItem> GetPermissionChild(Guid parentId, List<Permission> source)
    {
        return source.Where(p => p.ParentId == parentId).Select(p => new PermissionItem
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
        perimissionDetailQuery.Result = new PermissionDetail
        {

        };
    }
}
