// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.ApiGateways.Caller.Services.Logs;

public class OperationLogService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal OperationLogService(ICaller callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/operationLog/";
    }

    public async Task<PaginationDto<OperationLogDto>> GetListAsync(GetOperationLogsDto request)
    {
        return await SendAsync<GetOperationLogsDto, PaginationDto<OperationLogDto>>(nameof(GetListAsync), request);
    }

    public async Task<OperationLogDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, OperationLogDetailDto>(nameof(GetDetailAsync), new { id });
    }
}

