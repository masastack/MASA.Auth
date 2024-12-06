// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.


namespace Masa.Auth.Domain.Logs.Repositories;

public interface IOperationLogRepository : IRepository<OperationLog, Guid>
{
    Task AddDefaultAsync(OperationTypes operationType, string operationDescription, Guid? @operator = null);
}
