// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Projects;

public class OperationLogCommandHandler
{
    IOperationLogRepository _operationLogRepository;
    AuthDbContext _authDbContext;

    public OperationLogCommandHandler(IOperationLogRepository operationLogRepository, AuthDbContext authDbContext)
    {
        _operationLogRepository = operationLogRepository;
        _authDbContext = authDbContext;
    }

    [EventHandler]
    public async Task SaveAppTagOperationLogAysnc(UpsertAppTagCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.UpsertAppTag, $"添加应用标签：{command.AppTagDetail.Tag}");
    }
}
