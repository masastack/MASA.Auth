// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class OperationLogService : RestServiceBase
{
    public OperationLogService() : base("api/operationLog")
    {
        RouteHandlerBuilder = builder =>
        {
            builder.RequireAuthorization();
        };
    }

    private async Task<PaginationDto<OperationLogDto>> GetListAsync(IEventBus eventBus, GetOperationLogsDto operationLog)
    {
        var query = new OperationLogsQuery(operationLog.Page, operationLog.PageSize, operationLog.Operator, operationLog.OperationType, operationLog.StartTime, operationLog.EndTime, operationLog.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<OperationLogDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new OperationLogDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    /// <summary>
    /// Add operation log with client information
    /// </summary>
    private async Task AddAsync(IEventBus eventBus, [FromBody] OperationLogDto dto)
    {
        var command = new AddOperationLogCommand(
            dto.Operator,
            dto.OperatorName,
            dto.OperationType,
            dto.OperationTime,
            dto.OperationDescription,
            dto.ClientId);
        await eventBus.PublishAsync(command);
    }
}
