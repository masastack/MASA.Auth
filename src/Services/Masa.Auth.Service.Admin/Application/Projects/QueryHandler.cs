// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    private readonly IPmClient _pmClient;
    private readonly IDccClient _dccClient;
    private readonly IPermissionRepository _permissionRepository;
    private readonly UserDomainService _userDomainService;
    private readonly IMultiEnvironmentUserContext _multiEnvironmentUserContext;
    private readonly ILogger<QueryHandler> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IGlobalNavVisibleRepository _globalNavVisibleRepository;

    public QueryHandler(
        IPmClient pmClient,
        IPermissionRepository permissionRepository,
        UserDomainService userDomainService,
        IDccClient dccClient,
        IMultiEnvironmentUserContext multiEnvironmentUserContext,
        ILogger<QueryHandler> logger,
        IWebHostEnvironment webHostEnvironment,
        IGlobalNavVisibleRepository globalNavVisibleRepository)
    {
        _pmClient = pmClient;
        _permissionRepository = permissionRepository;
        _userDomainService = userDomainService;
        _dccClient = dccClient;
        _multiEnvironmentUserContext = multiEnvironmentUserContext;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment;
        _globalNavVisibleRepository = globalNavVisibleRepository;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentUserContext.Environment, AppTypes.UI, AppTypes.Service);
    }

    [EventHandler]
    public async Task GetProjectUIAppListAsync(ProjectUIAppListQuery query)
    {
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentUserContext.Environment, AppTypes.UI);
        var menuPermissions = await _permissionRepository.GetListAsync(p => p.Enabled && (p.Type == PermissionTypes.Menu
                || p.Type == PermissionTypes.Element));
        query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
        {
            a.Navs = menuPermissions.Where(p => p.AppId == a.Identity && p.GetParentId() == Guid.Empty)
            .OrderBy(p => p.Order).Select(p => new PermissionNavDto
            {
                Name = p.Name,
                Code = p.Id.ToString(),
                Children = GetChildren(p.Id, menuPermissions)
            }).ToList();
        });
    }

    [EventHandler]
    public async Task NavigationListQueryAsync(NavigationListQuery query)
    {
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentUserContext.Environment, AppTypes.UI);
        var permissionIds = await _userDomainService.GetPermissionIdsAsync(query.UserId);
        var menuPermissions = (await _permissionRepository.GetListAsync(p => p.Type == PermissionTypes.Menu
                                && permissionIds.Contains(p.Id) && p.Enabled)).ToList();

        if (!query.ClientId.IsNullOrEmpty())
        {
            await RemoveInvisibleMenu(query.ClientId, menuPermissions);
        }

        query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
        {
            a.Navs = menuPermissions.OrderBy(p => p.Order)
                .Where(p => p.AppId == a.Identity)
                .Where(p => p.GetParentId() == Guid.Empty)
                .Select(p => new PermissionNavDto
                {
                    Name = p.Name,
                    Icon = p.Icon,
                    Url = $"{a.Url?.EnsureTrailingSlash()}{p.Url?.TrimStart('/')}",
                    Code = p.Id.ToString(),
                    PermissionType = p.Type,
                    Children = GetChildren(p.Id, menuPermissions, a.Url ?? "")
                }).ToList();
        });
        //remove all empty item
        query.Result.RemoveAll(r => r.Apps.All(a => a.Navs.Count == 0));
    }

    private async Task<List<ProjectDto>> GetProjectDtoListAsync(string? env, params AppTypes[] appTypes)
    {
        if (env.IsNullOrEmpty())
        {
            env = _webHostEnvironment.EnvironmentName;
            _logger.LogWarning($"User context environment is empty,use system environment {env}");
        }

        var result = new List<ProjectDto>();
        var projects = await _pmClient.ProjectService.GetProjectAppsAsync(env);
        var tags = await _dccClient.LabelService.GetListByTypeCodeAsync("ProjectType");
        if (projects.Any())
        {
            result = projects.Select(p => new ProjectDto
            {
                Name = p.Name,
                Id = p.Id,
                Identity = p.Identity,
                Apps = p.Apps.Where(a => appTypes.Contains(a.Type))
                    .DistinctBy(a => a.Identity).Select(a => new AppDto
                    {
                        Name = a.Name,
                        Id = a.Id,
                        Tag = tags.FirstOrDefault(tag => tag.Code == p.LabelCode)?.Name ?? "Uncategorized",
                        Identity = a.Identity,
                        ProjectId = a.ProjectId,
                        Url = a.Url,
                        Type = a.Type
                    }).ToList()
            }).ToList();
        }
        return result;
    }

    private List<PermissionNavDto> GetChildren(Guid parentId, IEnumerable<Permission> all, string domain = "")
    {
        return all.Where(p => p.ParentId == parentId)
            .OrderBy(p => p.Order).Select(p => new PermissionNavDto
            {
                Name = p.Name,
                Icon = p.Icon,
                Url = $"{domain?.EnsureTrailingSlash()}{p.Url?.TrimStart('/')}",
                Code = p.Id.ToString(),
                PermissionType = p.Type,
                Children = GetChildren(p.Id, all, domain ?? "")
            }).ToList();
    }

    private async Task RemoveInvisibleMenu(string clientId, List<Permission> menuPermissions)
    {
        var appIds = menuPermissions.Select(p => p.AppId).Distinct();
        var appNavVisibles = await _globalNavVisibleRepository.GetListAsync(x => appIds.Contains(x.AppId));
        var hideAppIds = appNavVisibles.GroupBy(x => x.AppId).Where(x =>
        {
            if (x.Any(x => string.IsNullOrEmpty(x.ClientId) && x.Visible))
            {
                return false;
            }
            else if (x.Any(x => string.IsNullOrEmpty(x.ClientId) && !x.Visible))
            {
                return true;
            }
            else if (x.Count() > 0)
            {
                return !x.Any(x => x.ClientId == clientId);
            }
            return false;
        }).Select(x => x.Key);

        menuPermissions.RemoveAll(x => hideAppIds.Contains(x.AppId));
    }
}
