// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    readonly IPmClient _pmClient;
    readonly IDccClient _dccClient;
    readonly IPermissionRepository _permissionRepository;
    readonly UserDomainService _userDomainService;
    readonly IMultiEnvironmentContext _multiEnvironmentContext;

    public QueryHandler(
        IPmClient pmClient,
        IPermissionRepository permissionRepository,
        UserDomainService userDomainService,
        IDccClient dccClient,
        IMultiEnvironmentContext multiEnvironmentContext)
    {
        _pmClient = pmClient;
        _permissionRepository = permissionRepository;
        _userDomainService = userDomainService;
        _dccClient = dccClient;
        _multiEnvironmentContext = multiEnvironmentContext;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentContext.CurrentEnvironment, AppTypes.UI, AppTypes.Service);
    }

    [EventHandler]
    public async Task GetProjectUIAppListAsync(ProjectUIAppListQuery query)
    {
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentContext.CurrentEnvironment, AppTypes.UI);

        var menuPermissions = await _permissionRepository.GetListAsync(p => p.Type == PermissionTypes.Menu
                || p.Type == PermissionTypes.Element);
        query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
        {
            a.Navs = menuPermissions.Where(p => p.AppId == a.Identity && p.ParentId == Guid.Empty)
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
        query.Result = await GetProjectDtoListAsync(_multiEnvironmentContext.CurrentEnvironment, AppTypes.UI);

        var permissionIds = await _userDomainService.GetPermissionIdsAsync(query.UserId);
        var menuPermissions = await _permissionRepository.GetListAsync(p => p.Type == PermissionTypes.Menu
                                && permissionIds.Contains(p.Id) && p.Enabled);
        query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
        {
            a.Navs = menuPermissions.OrderBy(p => p.Order)
                .Where(p => p.AppId == a.Identity)
                .Where(p => p.ParentId == Guid.Empty)
                .Select(p => new PermissionNavDto
                {
                    Name = p.Name,
                    Icon = p.Icon,
                    Url = $"{a.Url?.TrimEnd('/')}/{p.Url?.TrimStart('/')}",
                    Code = p.Id.ToString(),
                    PermissionType = p.Type,
                    Children = GetChildren(p.Id, menuPermissions, a.Url ?? "")
                }).ToList();
        });
    }

    private async Task<List<ProjectDto>> GetProjectDtoListAsync(string env, params AppTypes[] appTypes)
    {
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
                Url = $"{domain?.TrimEnd('/')}/{p.Url?.TrimStart('/')}",
                Code = p.Id.ToString(),
                PermissionType = p.Type,
                Children = GetChildren(p.Id, all, domain ?? "")
            }).ToList();
    }
}
