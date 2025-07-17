// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Domain.Logs.Repositories;

public interface IOperationLogRepository : IRepository<OperationLog, Guid>
{
    /// <summary>
    /// Add default operation log
    /// </summary>
    Task AddDefaultAsync(OperationTypes operationType, string operationDescription, Guid? @operator = null);

    /// <summary>
    /// Add operation log
    /// </summary>
    Task AddDefaultAsync(OperationTypes operationType, string operationDescription, string? clientId, Guid? @operator = null);
}
