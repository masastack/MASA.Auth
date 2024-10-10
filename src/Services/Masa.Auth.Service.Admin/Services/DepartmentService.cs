// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class DepartmentService : ServiceBase
{
    public DepartmentService() : base("api/department")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };

        MapGet(GetAsync);
        MapGet(ListAsync);
        MapGet(CountAsync);
        MapPost(SaveAsync);
        MapPost(CopyAsync);
        MapDelete(RemoveAsync);
    }

    private async Task SaveAsync([FromServices] IEventBus eventBus,
        [FromBody] UpsertDepartmentDto upsertDepartmentDto)
    {
        await eventBus.PublishAsync(new UpsertDepartmentCommand(upsertDepartmentDto));
    }

    private async Task CopyAsync(IEventBus eventBus, [FromBody] CopyDepartmentDto copyDepartmentDto)
    {
        await eventBus.PublishAsync(new CopyDepartmentCommand(copyDepartmentDto));
    }

    private async Task<DepartmentDetailDto> GetAsync([FromServices] IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new DepartmentDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<DepartmentDto>> ListAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentTreeQuery(Guid.Empty);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task RemoveAsync([FromServices] IEventBus eventBus,
        [FromQuery] Guid id)
    {
        await eventBus.PublishAsync(new RemoveDepartmentCommand(id));
    }

    private async Task<DepartmentChildrenCountDto> CountAsync([FromServices] IEventBus eventBus)
    {
        var query = new DepartmentCountQuery();
        await eventBus.PublishAsync(query);
        return query.Result;
    }
}

