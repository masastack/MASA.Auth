// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects;

public class QueryHandler
{
    readonly IPmClient _pmClient;
    readonly IAppNavigationTagRepository _appNavigationTagRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly UserDomainService _userDomainService;

    public QueryHandler(IPmClient pmClient, IAppNavigationTagRepository appNavigationTagRepository,
            IPermissionRepository permissionRepository, UserDomainService userDomainService)
    {
        _pmClient = pmClient;
        _permissionRepository = permissionRepository;
        _appNavigationTagRepository = appNavigationTagRepository;
        _userDomainService = userDomainService;
    }

    [EventHandler]
    public async Task GetProjectListAsync(ProjectListQuery query)
    {
        var projects = await _pmClient.ProjectService.GetProjectAppsAsync(query.Environment);
        if (projects.Any())
        {
            var appTags = await _appNavigationTagRepository.GetListAsync();
            query.Result = projects.Select(p => new ProjectDto
            {
                Name = p.Name,
                Id = p.Id,
                Identity = p.Identity,
                Apps = p.Apps.DistinctBy(a => a.Identity).Select(a => new AppDto
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
    }

    [EventHandler]
    public async Task NavigationListQueryAsync(NavigationListQuery query)
    {
        var projects = await _pmClient.ProjectService.GetProjectAppsAsync(query.Environment);
        if (projects.Any())
        {
            var appTags = await _appNavigationTagRepository.GetListAsync();
            query.Result = projects.Select(p => new ProjectDto
            {
                Name = p.Name,
                Id = p.Id,
                Identity = p.Identity,
                Apps = p.Apps.DistinctBy(a => a.Identity).Select(a => new AppDto
                {
                    Name = a.Name,
                    Id = a.Id,
                    Tag = appTags.FirstOrDefault(at => at.AppIdentity == a.Identity)?.Tag ?? "",
                    Identity = a.Identity,
                    ProjectId = a.ProjectId
                }).ToList()
            }).ToList();
            var permissionIds = await _userDomainService.GetPermissionIdsAsync(query.UserId);
            var menuPermissions = await _permissionRepository.GetListAsync(p => p.Type == PermissionTypes.Menu
                                    && permissionIds.Contains(p.Id));
            query.Result.SelectMany(p => p.Apps).ToList().ForEach(a =>
            {
                a.Navs = menuPermissions.Where(p => p.AppId == a.Identity)
                    .Where(p => p.ParentId == Guid.Empty)
                    .Select(p => new PermissionNavDto
                    {
                        Name = p.Name,
                        Code = p.Id.ToString(),
                        Children = GetChildren(p.Id, menuPermissions)
                    }).ToList();
            });
        }
    }

    private List<PermissionNavDto> GetChildren(Guid parentId, IEnumerable<Permission> all)
    {
        return all.Where(p => p.ParentId == parentId).Select(p => new PermissionNavDto
        {
            Name = p.Name,
            Code = p.Id.ToString(),
            Children = GetChildren(p.Id, all)
        }).ToList();
    }

    [EventHandler]
    public async Task AppTagsAsync(AppTagsQuery query)
    {
#warning change dcc tag
        var tags = new List<string>() { "Tag1", "Tag2", "Tag3" };
        query.Result = tags;
        await Task.CompletedTask;
    }
}
