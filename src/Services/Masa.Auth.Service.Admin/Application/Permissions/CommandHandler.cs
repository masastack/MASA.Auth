﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class CommandHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly AuthDbContext _authDbContext;
    readonly RoleDomainService _roleDomainService;
    readonly PermissionDomainService _permissionDomainService;

    public CommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService,
        PermissionDomainService permissionDomainService)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
        _roleDomainService = roleDomainService;
        _permissionDomainService = permissionDomainService;
    }

    #region Role

    [EventHandler(1)]
    public async Task AddRoleAsync(AddRoleCommand command)
    {
        var roleDto = command.Role;
        var role = await _roleRepository.FindAsync(r => r.Name == roleDto.Name || r.Code == roleDto.Code);
        if (role is not null)
        {
            if (command.WhenExistReturn)
            {
                command.Result = role;
                return;
            }
            else
            {
                var error = "";
                if (role.Name == roleDto.Name) error += $"Role with Name {roleDto.Name} already exists;";
                if (role.Code == roleDto.Code) error += $"Role with Code {roleDto.Code} already exists;";
                if (error != "") throw new UserFriendlyException(error);
            }
        }

        role = new Role(roleDto.Name, roleDto.Code, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.AddAsync(role);
        await _roleRepository.UnitOfWork.SaveChangesAsync();
        command.Result = role;
    }

    [EventHandler(1)]
    public async Task UpdateRoleAsync(UpdateRoleCommand command)
    {
        var roleDto = command.Role;
        var role = await _roleRepository.FindAsync(r => r.Id != roleDto.Id && (r.Name == roleDto.Name || r.Code == roleDto.Code));
        if (role is not null)
        {
            var error = "";
            if (role.Name == roleDto.Name) error += $"Role with Name {roleDto.Name} already exists;";
            if (role.Code == roleDto.Code) error += $"Role with Code {roleDto.Code} already exists;";
            throw new UserFriendlyException(error);
        }

        role = await _roleRepository.GetByIdAsync(roleDto.Id);
        if (role is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);

        role.Update(roleDto.Name, roleDto.Code, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.UpdateAsync(role);
        // update and check role limit
        var influenceRoles = new List<Guid> { role.Id };
        await _roleDomainService.UpdateRoleLimitAsync(influenceRoles);
    }

    [EventHandler(1)]
    public async Task RemoveRoleAsync(RemoveRoleCommand command)
    {
        var role = await _authDbContext.Set<Role>()
                                    .Where(r => r.Id == command.Role.Id)
                                    .Include(r => r.ChildrenRoles)
                                    .Include(r => r.Permissions)
                                    .Include(r => r.Users)
                                    .Include(r => r.Teams)
                                    .AsSplitQuery()
                                    .FirstOrDefaultAsync();
        if (role is null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.ROLE_NOT_EXIST);

        await _roleRepository.RemoveAsync(role);
    }

    #endregion

    #region Permission

    [EventHandler(1)]
    public async Task RemovePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        var permission = await _permissionRepository.GetByIdAsync(removePermissionCommand.PermissionId);
        permission.DeleteCheck();
        await _permissionRepository.RemoveAsync(permission);
    }

    [EventHandler(1)]
    public async Task AddPermissionAsync(AddPermissionCommand addPermissionCommand)
    {
        var permissionBaseInfo = addPermissionCommand.PermissionDetail;

        Expression<Func<Permission, bool>> predicate = d => d.Code.Equals(permissionBaseInfo.Code) &&
            d.SystemId == permissionBaseInfo.SystemId && d.AppId == permissionBaseInfo.AppId;
        predicate = predicate.And(permissionBaseInfo.IsUpdate, d => d.Id != permissionBaseInfo.Id);

        if (_permissionRepository.Any(predicate))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.PERMISSION_CODE_EXIST, permissionBaseInfo.Code);
        }

        if (permissionBaseInfo.IsUpdate)
        {
            var _permission = await _permissionRepository.GetByIdAsync(permissionBaseInfo.Id);
            _permission.Update(permissionBaseInfo.AppId, permissionBaseInfo.Name,
                permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
                permissionBaseInfo.Description, permissionBaseInfo.Order, permissionBaseInfo.Enabled);
            _permission.SetParent(permissionBaseInfo.ParentId);
            _permission.BindApiPermission(permissionBaseInfo.ApiPermissions.ToArray());
            await _permissionRepository.UpdateAsync(_permission);
            return;
        }

        if (!_permissionDomainService.CanAdd(permissionBaseInfo.ParentId, permissionBaseInfo.Type))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.PERMISSION_PARENT_ADD_ERROR, permissionBaseInfo.Type);
        }

        if (permissionBaseInfo.Order == 0)
        {
            permissionBaseInfo.Order = _permissionRepository.GetIncrementOrder(permissionBaseInfo.AppId, permissionBaseInfo.ParentId);
        }
        var permission = new Permission(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
            permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
            permissionBaseInfo.Description, permissionBaseInfo.Order, permissionBaseInfo.Enabled);
        permission.SetParent(permissionBaseInfo.ParentId);
        permission.BindApiPermission(permissionBaseInfo.ApiPermissions.ToArray());
        await _permissionRepository.AddAsync(permission);
        await _permissionRepository.UnitOfWork.SaveChangesAsync();
        addPermissionCommand.PermissionDetail.Id = permission.Id;
    }

    [EventHandler(1)]
    public async Task SeedPermissionsAsync(SeedPermissionsCommand seedPermissionsCommand)
    {
        foreach (var permission in seedPermissionsCommand.Permissions)
        {
            await _permissionRepository.AddAsync(permission);
        }
        await _permissionRepository.UnitOfWork.SaveChangesAsync();
    }
    #endregion
}