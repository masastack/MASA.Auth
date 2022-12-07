// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class OperationLogCommandHandler
{
    IOperationLogRepository _operationLogRepository;
    AuthDbContext _authDbContext;

    public OperationLogCommandHandler(IOperationLogRepository operationLogRepository, AuthDbContext authDbContext)
    {
        _operationLogRepository = operationLogRepository;
        _authDbContext = authDbContext;
    }

    #region Role

    [EventHandler(1)]
    public async Task AddRoleOperationLogAsync(AddRoleCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.AddRole, $"添加角色：{command.Role.Name}");
    }

    [EventHandler(1)]
    public async Task UpdateRoleOperationLogAsync(UpdateRoleCommand command)
    {
        await _operationLogRepository.AddDefaultAsync(OperationTypes.EditRole, $"编辑角色：{command.Role.Name}");
    }

    [EventHandler(0)]
    public async Task RemoveRoleOperationLogAsync(RemoveRoleCommand command)
    {
        var name = await _authDbContext.Set<Role>()
                                              .Where(role => role.Id == command.Role.Id)
                                              .Select(role => role.Name)
                                              .FirstAsync();
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemoveRole, $"删除角色：{name}");
    }

    #endregion

    #region Permission

    [EventHandler(1)]
    public async Task RemovePermissionOperationLogAsync(RemovePermissionCommand command)
    {
        var name = await _authDbContext.Set<Permission>()
                                    .Where(permission => permission.Id == command.PermissionId)
                                    .Select(permission => permission.Name)
                                    .FirstAsync();
        if (name is not null)
            await _operationLogRepository.AddDefaultAsync(OperationTypes.RemovePermission, $"删除权限：{name}");
    }

    [EventHandler(1)]
    public async Task AddPermissionOperationLogAsync(AddPermissionCommand command)
    {
        if(command.PermissionDetail.IsUpdate)
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.EditPermission, $"编辑权限：{command.PermissionDetail.Name}");
        }
        else
        {
            await _operationLogRepository.AddDefaultAsync(OperationTypes.AddPermission, $"添加权限：{command.PermissionDetail.Name}");
        }
    }

    #endregion
}
