// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs;

public class QueryHandler
{
    readonly IOperationLogRepository _operationLogRepository;

    public QueryHandler(IOperationLogRepository operationLogRepository)
    {
        _operationLogRepository = operationLogRepository;
    }

    [EventHandler]
    public async Task GetOperationLogAsync(OperationLogsQuery query)
    {
        Expression<Func<OperationLog, bool>> condition = operationLog => true;
        if (query.Operator != default)
            condition = condition.And(operationLog => operationLog.Operator == query.Operator);
        if (query.OperationType != default)
            condition = condition.And(operationLog => operationLog.OperationType == query.OperationType);
        if (query.StartTime is not null)
            condition = condition.And(operationLog => operationLog.OperationTime >= query.StartTime);
        if (query.EndTime is not null)
            condition = condition.And(operationLog => operationLog.OperationTime <= query.EndTime);
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(operationLog =>
                            operationLog.OperationDescription.Contains(query.Search) ||
                            operationLog.OperatorName.Contains(query.Search)
                        );

        var operationLogs = await _operationLogRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(OperationLog.OperationTime)] = true,
            }
        });

        query.Result = new(operationLogs.Total, operationLogs.Result.Select(operationLog =>
            operationLog.Adapt<OperationLogDto>()
        ).ToList());
    }

    [EventHandler]
    public async Task GetOperationLogDetailAsync(OperationLogDetailQuery query)
    {
        var operationLog = await _operationLogRepository.FindAsync(query.OperationLogId);
        if (operationLog is null) throw new UserFriendlyException("This operationLog data does not exist");

        query.Result = operationLog.Adapt<OperationLogDetailDto>();
    }
}

