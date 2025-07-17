// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Domain.Logs.Repositories;

public interface IOperationLogRepository : IRepository<OperationLog, Guid>
{
    /// <summary>
    /// 添加默认操作日志
    /// </summary>
    Task AddDefaultAsync(OperationTypes operationType, string operationDescription, Guid? @operator = null);

    /// <summary>
    /// 添加操作日志
    /// </summary>
    Task AddDefaultAsync(OperationTypes operationType, string operationDescription, string? clientId, Guid? @operator = null);
}
