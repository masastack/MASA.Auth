using Masa.Auth.Service.Admin.Application.Subjects.Commands;
using Masa.Auth.Service.Admin.Application.Subjects.Queries;
using Masa.BuildingBlocks.Dispatcher.Events;
using Microsoft.AspNetCore.Mvc;

namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class TeamService : ServiceBase
{
    string _baseUrl = "api/staff";

    internal TeamService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<List<TeamDto>> ListAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new TeamListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task<TeamDetailDto> GetAsync(Guid id)
    {
        return await Task.FromResult(new TeamDetailDto());
    }

    public async Task<List<TeamSelectDto>> SelectAsync(IEventBus eventBus)
    {
        var query = new TeamSelectListQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task AddAsync(AddTeamDto request)
    {

    }

    public async Task UpdateAsync(UpdateTeamDto request)
    {

    }

    public async Task DeleteAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var removeCommand = new RemoveTeamCommand(id);
        await eventBus.PublishAsync(removeCommand);
    }
}
