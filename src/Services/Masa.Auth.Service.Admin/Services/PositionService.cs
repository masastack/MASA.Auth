// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class PositionService : RestServiceBase
{
    public PositionService(IServiceCollection services) : base(services, "api/position")
    {

    }

    private async Task<StaffDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new StaffDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<PositionSelectDto>> GetSelectAsync(IEventBus eventBus, string name)
    {
        var query = new PositionSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task UpsertAsync(IEventBus eventBus,
        [FromBody] UpsertPositionDto position)
    {
        await eventBus.PublishAsync(new UpsertPositionCommand(position));
    }
}
