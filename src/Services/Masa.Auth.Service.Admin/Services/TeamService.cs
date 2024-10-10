// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class TeamService : ServiceBase
{
    public TeamService() : base("api/team")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapGet(GetAsync);
        MapGet(GetDetailForExternalAsync, "detail");
        MapGet(ListAsync);
        MapGet(SelectAsync);
        MapGet(GetTeamRoleSelectAsync);
        MapPost(CreateAsync);
        MapPost(UpdateAsync);
        MapDelete(RemoveAsync);
    }

    [MasaAuthorize]
    private async Task CreateAsync(IEventBus eventBus, [FromBody] AddTeamDto addTeamDto)
    {
        await eventBus.PublishAsync(new AddTeamCommand(addTeamDto));
    }

    private async Task UpdateAsync(IEventBus eventBus, [FromBody] UpdateTeamDto updateTeamDto)
    {
        await eventBus.PublishAsync(new UpdateTeamCommand(updateTeamDto));
    }

    private async Task<TeamDetailDto> GetAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new TeamDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<TeamDetailModel?> GetDetailForExternalAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new TeamDetailForExternalQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamDto>> ListAsync(IEventBus eventBus, IMultiEnvironmentSetter environmentSetter, [FromQuery] string? name, [FromQuery] Guid? userId, [FromQuery] string? environment)
    {
        //todo add middleware
        if (!environment.IsNullOrEmpty())
        {
            environmentSetter.SetEnvironment(environment);
        }
        var query = new TeamListQuery(name ?? "", userId ?? Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamSelectDto>> SelectAsync(IEventBus eventBus, [FromQuery] string name)
    {
        var query = new TeamSelectListQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamRoleSelectDto>> GetTeamRoleSelectAsync(IEventBus eventBus, [FromQuery] string name, [FromQuery] Guid UserId)
    {
        var query = new TeamRoleSelectQuery(name, UserId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveTeamCommand(id));
    }
}
