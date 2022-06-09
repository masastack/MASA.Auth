// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class TeamService : ServiceBase
{
    public TeamService(IServiceCollection services) : base(services, "api/team")
    {
        MapGet(GetAsync);
        MapGet(GetDetailForExternalAsync);
        MapGet(ListAsync);
        MapGet(SelectAsync);
        MapPost(CreateAsync);
        MapPost(UpdateBasicInfoAsync);
        MapPost(UpdateAdminPersonnelAsync);
        MapPost(UpdateMemberPersonnelAsync);
        MapDelete(RemoveAsync);
    }

    private async Task CreateAsync(IEventBus eventBus, [FromBody] AddTeamDto addTeamDto)
    {
        await eventBus.PublishAsync(new AddTeamCommand(addTeamDto));
    }

    private async Task UpdateBasicInfoAsync(IEventBus eventBus, [FromBody] UpdateTeamBasicInfoDto updateTeamBasicInfoDto)
    {
        await eventBus.PublishAsync(new UpdateTeamBasicInfoCommand(updateTeamBasicInfoDto));
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

    private async Task<TeamDetailForExternalDto> GetDetailForExternalAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new TeamDetailForExternalQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<TeamDto>> ListAsync(IEventBus eventBus, [FromQuery] string? name, [FromQuery] Guid? userId)
    {
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

    private async Task RemoveAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveTeamCommand(id));
    }
}
