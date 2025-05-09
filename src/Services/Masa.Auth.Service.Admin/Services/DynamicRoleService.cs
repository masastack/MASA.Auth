// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class DynamicRoleService : ServiceBase
{
    public DynamicRoleService() : base("api/dynamic-role")
    {
        RouteOptions.DisableAutoMapRoute = false;
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    [RoutePattern("", StartWithBaseUri = true, HttpMethod = "Get")]
    public async Task<PaginationDto<DynamicRoleDto>> GetPageAsync(IEventBus eventbus, GetDynamicRoleInput input)
    {
        var query = new DynamicRolePageQuery(input);
        await eventbus.PublishAsync(query);
        return query.Result;
    }

    public async Task<DynamicRoleDto> GetAsync(IEventBus eventBus, Guid id)
    {
        var query = new DynamicRoleQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    public async Task CreateAsync(IEventBus eventBus, [FromBody] DynamicRoleUpsertDto input)
    {
        var command = new CreateDynamicRoleCommand(input);
        await eventBus.PublishAsync(command);
    }

    public async Task UpdateAsync(IEventBus eventBus, Guid id, [FromBody] DynamicRoleUpsertDto input)
    {
        var command = new UpdateDynamicRoleCommand(id, input);
        await eventBus.PublishAsync(command);
    }

    public async Task DeleteAsync(IEventBus eventBus, Guid id)
    {
        var command = new DeleteDynamicRoleCommand(id);
        await eventBus.PublishAsync(command);
    }

    [RoutePattern("{id}/validate", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<List<DynamicRoleValidateDto>> ValidateAsync(IEventBus eventBus, Guid id)
    {
        var command = new ValidateDynamicRoleCommand(new List<Guid> { id });
        await eventBus.PublishAsync(command);
        return command.Result;
    }

    [RoutePattern("validate", StartWithBaseUri = true, HttpMethod = "Post")]
    public async Task<List<DynamicRoleValidateDto>> ValidateAsync(IEventBus eventBus, DynamicRoleValidateInput input)
    {
        var command = new ValidateDynamicRoleCommand(input.RoleIds);
        await eventBus.PublishAsync(command);
        return command.Result;
    }
}
