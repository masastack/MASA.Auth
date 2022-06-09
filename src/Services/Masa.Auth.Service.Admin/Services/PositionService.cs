// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class PositionService : RestServiceBase
{
    public PositionService(IServiceCollection services) : base(services, "api/position")
    {

    }

    private async Task<PaginationDto<PositionDto>> GetListAsync(IEventBus eventBus, GetPositionsDto position)
    {
        var query = new PositionsQuery(position.Page, position.PageSize, position.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<PositionDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new PositionDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<List<PositionSelectDto>> GetSelectAsync(IEventBus eventBus, string name)
    {
        var query = new PositionSelectQuery(name);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task AddAsync(IEventBus eventBus,
        [FromBody] AddPositionDto position)
    {
        await eventBus.PublishAsync(new AddPositionCommand(position));
    }

    private async Task UpdateAsync(IEventBus eventBus,
        [FromBody] UpdatePositionDto position)
    {
        await eventBus.PublishAsync(new UpdatePositionCommand(position));
    }

    private async Task RemoveAsync(
            IEventBus eventBus,
            [FromBody] RemovePositionDto dto)
    {
        await eventBus.PublishAsync(new RemovePositionCommand(dto));
    }
}
