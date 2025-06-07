// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs;

public class QueryHandler
{
    readonly IOperationLogRepository _operationLogRepository;
    readonly IStaffRepository _staffRepository;

    public QueryHandler(IOperationLogRepository operationLogRepository, IStaffRepository staffRepository)
    {
        _operationLogRepository = operationLogRepository;
        _staffRepository = staffRepository;
    }

    [EventHandler]
    public async Task GetOperationLogAsync(OperationLogsQuery query)
    {
        var startTime = query.StartTime.HasValue
            ? DateTime.SpecifyKind(query.StartTime.Value, DateTimeKind.Utc)
            : (DateTime?)null;
        var endTime = query.EndTime.HasValue
            ? DateTime.SpecifyKind(query.EndTime.Value, DateTimeKind.Utc)
            : (DateTime?)null;

        Expression<Func<OperationLog, bool>> condition = operationLog => true;
        condition = condition.And(query.Operator != default, operationLog => operationLog.Operator == query.Operator);
        condition = condition.And(query.OperationType != default, operationLog => operationLog.OperationType == query.OperationType);
        condition = condition.And(startTime is not null, operationLog => operationLog.OperationTime >= startTime);
        condition = condition.And(endTime is not null, operationLog => operationLog.OperationTime <= endTime);
        condition = condition.And(!string.IsNullOrEmpty(query.Search), operationLog =>
                        operationLog.OperationDescription.Contains(query.Search) ||
                        operationLog.OperatorName.Contains(query.Search));

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

        var operatorIds = query.Result.Items.Select(o => o.Operator);
        var staffs = await _staffRepository.GetListAsync(staff => operatorIds.Contains(staff.UserId));

        query.Result.Items.ForEach(item =>
        {
            var staff = staffs.FirstOrDefault(staff => staff?.UserId == item.Operator);
            if (staff is not null && !string.IsNullOrEmpty(staff.DisplayName))
            {
                item.OperatorName = staff.DisplayName;
            }
        });
    }

    [EventHandler]
    public async Task GetOperationLogDetailAsync(OperationLogDetailQuery query)
    {
        var operationLog = await _operationLogRepository.FindAsync(query.OperationLogId);
        if (operationLog is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.OPERATION_LOG_NOT_EXIST);

        query.Result = operationLog.Adapt<OperationLogDetailDto>();
    }
}

