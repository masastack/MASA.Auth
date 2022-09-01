// Copyright (c) MASA Stack All rights reserved.
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
        if (await _roleRepository.GetCountAsync(u => u.Name == roleDto.Name) > 0)
            throw new UserFriendlyException($"Role with Name {roleDto.Name} already exists");

        var role = new Role(roleDto.Name, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.AddAsync(role);
        await _roleRepository.UnitOfWork.SaveChangesAsync();
        command.RoleId = role.Id;
    }

    [EventHandler(1)]
    public async Task UpdateRoleAsync(UpdateRoleCommand command)
    {
        var roleDto = command.Role;
        var role = await _roleRepository.GetByIdAsync(roleDto.Id);
        if (role is null)
            throw new UserFriendlyException($"The current role does not exist");

        role.Update(roleDto.Name, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.UpdateAsync(role);
        //await _roleRepository.UnitOfWork.SaveChangesAsync();
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
            throw new UserFriendlyException($"The current role does not exist");

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
        if (permissionBaseInfo.IsUpdate)
        {
            predicate = predicate.And(d => d.Id != permissionBaseInfo.Id);
        }
        if (_permissionRepository.Any(predicate))
        {
            throw new UserFriendlyException($"The permission code {permissionBaseInfo.Code} already exists");
        }

        if (!_permissionDomainService.CanAdd(addPermissionCommand.ParentId, permissionBaseInfo.Type))
        {
            throw new UserFriendlyException($"The current parent doesn't support add {permissionBaseInfo.Type} type permission, conflicts with other permission type");
        }

        if (permissionBaseInfo.IsUpdate)
        {
            var _permission = await _permissionRepository.GetByIdAsync(permissionBaseInfo.Id);
            _permission.Update(permissionBaseInfo.AppId, permissionBaseInfo.Name,
                permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
                permissionBaseInfo.Description, permissionBaseInfo.Order, addPermissionCommand.Enabled);
            _permission.SetParent(addPermissionCommand.ParentId);
            _permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
            await _permissionRepository.UpdateAsync(_permission);
            return;
        }

        if (permissionBaseInfo.Order == 0)
        {
            permissionBaseInfo.Order = _permissionRepository.GetIncrementOrder(permissionBaseInfo.AppId, addPermissionCommand.ParentId);
        }
        var permission = new Permission(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
            permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
            permissionBaseInfo.Description, permissionBaseInfo.Order, addPermissionCommand.Enabled);
        permission.SetParent(addPermissionCommand.ParentId);
        permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
        await _permissionRepository.AddAsync(permission);
        await _permissionRepository.UnitOfWork.SaveChangesAsync();
    }

    #endregion
}