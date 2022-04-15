namespace Masa.Auth.Service.Admin.Services;

public class ProjectService : ServiceBase
{
    public ProjectService(IServiceCollection services) : base(services, "api/project")
    {
        MapGet(GetList);
    }

    private async Task<List<ProjectDto>> GetList(IEventBus eventBus)
    {
        var query = new ProjectListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}
