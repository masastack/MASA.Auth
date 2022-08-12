// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class RoleService : RestServiceBase
{
    readonly ILogger<RoleService> _logger;
    public RoleService(IServiceCollection services, ILogger<RoleService> logger) : base(services, "api/role")
    {
        _logger = logger;
    }

    private async Task<PaginationDto<RoleDto>> GetListAsync([FromServices] IEventBus eventBus, GetRolesDto role)
    {
        var query = new GetRolesQuery(role.Page, role.PageSize, role.Search, role.Enabled);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetTopRoleSelectAsync([FromServices] IEventBus eventBus, Guid roleId)
    {
        var query = new TopRoleSelectQuery(roleId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetSelectForUserAsync([FromServices] IEventBus eventBus, Guid userId)
    {
        var query = new RoleSelectForUserQuery(userId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetSelectForRoleAsync([FromServices] IEventBus eventBus, Guid roleId)
    {
        var query = new RoleSelectForRoleQuery(roleId);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<RoleSelectDto>> GetSelectForTeamAsync([FromServices] IEventBus eventBus, Guid teamId, TeamMemberTypes teamMemberType)
    {
        var query = new RoleSelectForTeamQuery(teamId, teamMemberType);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<RoleDetailDto> GetDetailAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new RoleDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<RoleOwnerDto> GetRoleOwnerAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new RoleOwnerQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] AddRoleDto role)
    {
        await eventBus.PublishAsync(new AddRoleCommand(role));
    }

    private async Task UpdateAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] UpdateRoleDto role)
    {
        await eventBus.PublishAsync(new UpdateRoleCommand(role));
    }

    private async Task RemoveAsync(
        [FromServices] IEventBus eventBus,
        [FromBody] RemoveRoleDto role)
    {
        await eventBus.PublishAsync(new RemoveRoleCommand(role));
    }
}

