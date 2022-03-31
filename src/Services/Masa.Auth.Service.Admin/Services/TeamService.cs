namespace Masa.Auth.Service.Admin.Services;

public class TeamService : ServiceBase
{
    public TeamService(IServiceCollection services) : base(services, "api/team")
    {
        MapGet(GetAsync);
        MapGet(ListAsync);
        MapGet(SelectAsync);
        MapPost(CreateAsync);
        MapPost(UpdateBaseInfoAsync);
        MapPost(UpdateAdminPersonnelAsync);
        MapPost(UpdateMemberPersonnelAsync);
        MapDelete(RemoveAsync);
    }

    private async Task CreateAsync(IEventBus eventBus, [FromBody] AddTeamDto addTeamDto)
    {
        await eventBus.PublishAsync(new AddTeamCommand(addTeamDto));
    }

    private async Task UpdateBaseInfoAsync(IEventBus eventBus, [FromBody] UpdateTeamBaseInfoDto updateTeamBaseInfoDto)
    {
        await eventBus.PublishAsync(new UpdateTeamBaseInfoCommand(updateTeamBaseInfoDto));
    }

    private async Task UpdateAdminPersonnelAsync(IEventBus eventBus, [FromBody] UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await eventBus.PublishAsync(new UpdateTeamPersonnelCommand(updateTeamPersonnelDto, TeamMemberTypes.Admin));
    }

    private async Task UpdateMemberPersonnelAsync(IEventBus eventBus, [FromBody] UpdateTeamPersonnelDto updateTeamPersonnelDto)
    {
        await eventBus.PublishAsync(new UpdateTeamPersonnelCommand(updateTeamPersonnelDto, TeamMemberTypes.Member));
    }

    private async Task<TeamDetailDto> GetAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new TeamDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamDto>> ListAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new TeamListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamSelectDto>> SelectAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new TeamSelectListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveTeamCommand(id));
    }
}
