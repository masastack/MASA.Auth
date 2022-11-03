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

    private async Task<List<ProjectDto>> GetNavigationListAsync(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new NavigationListQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
