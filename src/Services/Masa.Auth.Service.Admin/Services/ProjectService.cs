using Masa.Auth.Service.Admin.Application.Projects.Commands;

namespace Masa.Auth.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService(IServiceCollection services) : base(services, "api/project")
    {
        MapGet(GetListAsync);
        MapGet(GetTagsAsync);
        MapPost(SaveAppTagAsync);
    }

    private async Task<List<ProjectDto>> GetListAsync(IEventBus eventBus)
    {
        var query = new ProjectListQuery();
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
