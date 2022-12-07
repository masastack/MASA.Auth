// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations;

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
    public async Task UpsertDepartmentOperationLogAsync(UpsertDepartmentCommand command)
    {
        if (command.UpsertDepartmentDto.Id == Guid.Empty)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddDepartment, $"新建部门：{command.UpsertDepartmentDto.Name}");
        else
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditDepartment, $"编辑部门：{command.UpsertDepartmentDto.Name}");
    }

    [EventHandler]
    public async Task CopyDepartmentOperationLogAsync(CopyDepartmentCommand command)
    {
        var name = await _authDbContext.Set<Department>()
                                              .Where(department => department.Id == command.CopyDepartmentDto.SourceId)
                                              .Select(department => department.Name)
                                              .FirstAsync();
        await _operationLogRepository.AddDefaultAsync(OperationTypes.CopyDepartment, $"复制部门：{name}为{command.CopyDepartmentDto.Name}");
    }

    [EventHandler(0)]
    public async Task RemoveDepartmentOperationLogAsync(RemoveDepartmentCommand command)
    {
        var name = await _authDbContext.Set<Department>()
                                              .Where(department => department.Id == command.DepartmentId)
                                              .Select(department => department.Name)
                                              .FirstAsync();
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveDepartment, $"删除部门：{name}");
    }

    [EventHandler]
    public async Task AddPositionOperationLogAsync(AddPositionCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddPosition, $"新建职位：{command.Position.Name}");
    }

    [EventHandler]
    public async Task UpdatePositionOperationLogAsync(UpdatePositionCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.EditPosition, $"编辑职位：{command.Position.Name}");
    }

    [EventHandler(0)]
    public async Task RemovePositionOperationLogAsync(RemovePositionCommand command)
    {
        var name = await _authDbContext.Set<Position>()
                                    .Where(position => position.Id == command.Position.Id)
                                    .Select(position => position.Name)
                                    .FirstAsync();
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemovePosition, $"删除职位：{name}");
    }
}
