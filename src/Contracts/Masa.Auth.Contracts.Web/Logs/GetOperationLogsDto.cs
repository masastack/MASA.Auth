// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Logs;

public class GetOperationLogsDto : Pagination
{
    public Guid Operator { get; set; }

    public DateTime? StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public OperationTypes OperationType { get; set; }

    public string Search { get; set; }

    public GetOperationLogsDto(int page, int pageSize, Guid @operator, DateTime? startTime, DateTime? endTime, OperationTypes operationType, string search)
    {
        Page = page;
        PageSize = pageSize;
        Operator = @operator;
        StartTime = startTime;
        EndTime = endTime;
        Search = search;
        OperationType = operationType;
    }
}

