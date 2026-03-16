// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService() : base("api/project")
    {
        MapGet(GetListAsync);
        MapGet(GetUIAndMenusAsync);
        MapGet(GetNavigationListAsync, "navigations");

        MapGet(GetMenuDetailAsync, "menus/detail");
        MapPut(UpdateMenuMetaAsync, "menus/meta").RequireAuthorization();
        MapGet(GetProjectListByAppIdsAsync, "byAppIds");
    }

    private async Task<List<ProjectDto>> GetListAsync(IEventBus eventBus)
    {
        var query = new ProjectListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ProjectDto>> GetUIAndMenusAsync(IEventBus eventBus)
    {
        var query = new ProjectUIAppListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ProjectDto>> GetNavigationListAsync(IEventBus eventBus, [FromQuery] Guid userId, [FromQuery] string? clientId)
    {
        var query = new NavigationListQuery(userId, clientId ?? string.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PermissionNavDetailDto> GetMenuDetailAsync(IEventBus eventBus, [FromQuery] Guid menuId)
    {
        var query = new PermissionNavDetailQuery(menuId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task UpdateMenuMetaAsync(IEventBus eventBus, [FromBody] UpdateMenuMetaDto dto)
    {
        var command = new UpdateMenuMetaCommand(dto);
        await eventBus.PublishAsync(command);
    }

    private async Task<List<ProjectDto>> GetProjectListByAppIdsAsync(
        IEventBus eventBus,
        HttpRequest request)
    {
        var appIds = request.Query["appIds"].ToArray();
        var query = new ProjectListByAppIdsQuery(appIds);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
