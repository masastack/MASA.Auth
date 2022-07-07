// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Logs;

public class CommandHandler
{
    readonly IOperationLogRepository _operationLogRepository;

    public CommandHandler(IOperationLogRepository operationLogRepository)
    {
        _operationLogRepository = operationLogRepository;
    }

    [EventHandler]
    public async Task AddOperationLogAsync(AddOperationLogCommand command)
    {
        await _operationLogRepository.AddAsync(command.Adapt<OperationLog>());
    }

    [EventHandler]
    public async Task RemoveOperationLogAsync(RemoveOperationLogCommand command)
    {
        var operationLog = await _operationLogRepository.FindAsync(command.Id);
        if (operationLog == null)
        {
            throw new UserFriendlyException("the current operationLog not found");
        }
        await _operationLogRepository.RemoveAsync(operationLog);
    }
}

