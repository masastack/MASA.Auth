// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService() : base("api/project")
    {
        MapGet(GetListAsync);
        MapGet(GetNavigationListAsync, "navigations");
        MapGet(GetTagsAsync);
        MapPost(SaveAppTagAsync);
    }

    private async Task<List<ProjectDto>> GetListAsync(IEventBus eventBus, [FromQuery] bool hasMenu = false)
    {
        var query = new ProjectListQuery(hasMenu);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<ProjectDto>> GetNavigationListAsync(IEventBus eventBus, [FromQuery] Guid userId)
    {
        var query = new NavigationListQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<string>> GetTagsAsync(IEventBus eventBus)
    {
        var query = new AppTagsQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task SaveAppTagAsync(IEventBus eventBus, [FromBody] AppTagDetailDto dto)
    {
        var upsertCommand = new UpsertAppTagCommand(dto);
        await eventBus.PublishAsync(upsertCommand);
    }
}
