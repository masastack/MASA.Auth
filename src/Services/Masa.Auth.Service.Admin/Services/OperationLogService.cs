// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Services;

public class OperationLogService : RestServiceBase
{
    public OperationLogService(IServiceCollection services) : base(services, "api/operationLog")
    {

    }

    private async Task<PaginationDto<OperationLogDto>> GetListAsync(IEventBus eventBus, GetOperationLogsDto operationLog)
    {
        var query = new OperationLogsQuery(operationLog.Page, operationLog.PageSize, operationLog.Operator, operationLog.StartTime, operationLog.EndTime, operationLog.Search);
        await eventBus.PublishAsync(query);
        return query.Result;
    }

    private async Task<OperationLogDetailDto> GetDetailAsync(IEventBus eventBus, [FromQuery] Guid id)
    {
        var query = new OperationLogDetailQuery(id);
        await eventBus.PublishAsync(query);
        return query.Result;
    }   
}
