namespace Masa.Auth.Service.Admin.Services;

public class TeamService : ServiceBase
{
    public TeamService(IServiceCollection services) : base(services, "api/team")
    {
        MapGet(GetAsync);
        MapGet(ListAsync);
        MapDelete(RemoveAsync);
    }

    //private async Task AddAsync([FromServices] IEventBus eventBus,
    //    [FromBody] AddOrUpdateDepartmentDto addOrUpdateDepartmentDto)
    //{
    //    await eventBus.PublishAsync(new AddDepartmentCommand(addOrUpdateDepartmentDto));
    //}

    private async Task<TeamDetailDto> GetAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new TeamDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamDto>> ListAsync([FromServices] IEventBus eventBus, [FromQuery] string name)
    {
        var query = new TeamListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveTeamCommand(id));
    }
}
