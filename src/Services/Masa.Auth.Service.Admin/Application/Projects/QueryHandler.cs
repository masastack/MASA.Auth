namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    readonly IPmClient _pmClient;
    readonly IAppNavigationTagRepository _appNavigationTagRepository;
    readonly IPermissionRepository _permissionRepository;

    public QueryHandler(IPmClient pmClient, IAppNavigationTagRepository appNavigationTagRepository,
            IPermissionRepository permissionRepository)
    {
        _pmClient = pmClient;
        _permissionRepository = permissionRepository;
        _appNavigationTagRepository = appNavigationTagRepository;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        var projects = await _pmClient.ProjectService.GetProjectListAsync("development");
        if (projects.Any())
        {
            var appTags = await _appNavigationTagRepository.GetListAsync();
            query.Result = projects.Select(p => new ProjectDto
            {
                Name = p.Name,
                Id = p.Id,
                Identity = p.Identity,
                Apps = p.Apps.Select(a => new AppDto
                {
                    Name = a.Name,
                    Id = a.Id,
                    Tag = appTags.FirstOrDefault(at => at.AppIdentity == a.Identity)?.Tag ?? "",
                    Identity = a.Identity,
                    ProjectId = a.ProjectId
                }).ToList()
            }).ToList();

            if (query.HasMenu)
            {
                var menuPermissions = await _permissionRepository.GetListAsync(p => p.Type == PermissionTypes.Menu
                    || p.Type == PermissionTypes.Element);
                query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
                {
                    a.Navs = menuPermissions.Where(p => p.AppId == a.Identity).Where(p => p.ParentId == Guid.Empty).Select(p => new PermissionNavDto
                    {
                        Name = p.Name,
                        Code = p.Id.ToString(),
                        Children = GetChildren(p.Id, menuPermissions)
                    }).ToList();
                });
            }
        }
        await Task.CompletedTask;
    }

    private List<PermissionNavDto> GetChildren(Guid parentId, IEnumerable<Permission> all)
    {
        return all.Where(p => p.ParentId == parentId).Select(p => new PermissionNavDto
        {
            Name = p.Name,
            Code = p.Code,
            Children = GetChildren(p.Id, all)
        }).ToList();
    }

    [EventHandler]
    public async Task AppTagsAsync(AppTagsQuery query)
    {
        var tags = new List<string>() { "Tag1", "Tag2", "Tag3" };
        query.Result = tags;
        await Task.CompletedTask;
    }
}
