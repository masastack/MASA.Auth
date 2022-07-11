// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs.Queries;

public record OperationLogsQuery(
    int Page,
    int PageSize,
    Guid Operator,
    DateTime? StartTime,
    DateTime? EndTime,
    string Search
    ) : Query<PaginationDto<OperationLogDto>>
{
    public override PaginationDto<OperationLogDto> Result { get; set; } = new();
}
