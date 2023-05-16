// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Permissions;

public class CommandHandler
{
    private readonly IRoleRepository _roleRepository;
    private readonly IPermissionRepository _permissionRepository;
    private readonly AuthDbContext _authDbContext;
    private readonly RoleDomainService _roleDomainService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly PermissionDomainService _permissionDomainService;

    public CommandHandler(
        IRoleRepository roleRepository,
        IPermissionRepository permissionRepository,
        AuthDbContext authDbContext,
        RoleDomainService roleDomainService,
        IUnitOfWork unitOfWork,
        PermissionDomainService permissionDomainService)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _authDbContext = authDbContext;
        _unitOfWork = unitOfWork;
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
        await _unitOfWork.SaveChangesAsync();
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
        await RemovePermissionLocalAsync(removePermissionCommand.PermissionId);

        async Task RemovePermissionLocalAsync(Guid permissionId)
        {
            var permission = await _permissionRepository.GetByIdAsync(permissionId);
            permission.DeleteCheck();
            await _permissionRepository.RemoveAsync(permission);
            foreach (var child in permission.Children)
            {
                await RemovePermissionLocalAsync(child.Id);
            }
        }
    }

    [EventHandler(1)]
    public async Task UpsertPermissionAsync(UpsertPermissionCommand upsertPermissionCommand)
    {
        var permissionBaseInfo = upsertPermissionCommand.PermissionDetail;

        Expression<Func<Permission, bool>> predicate = d => d.Code.Equals(permissionBaseInfo.Code) &&
            d.SystemId == permissionBaseInfo.SystemId && d.AppId == permissionBaseInfo.AppId;
        predicate = predicate.And(permissionBaseInfo.IsUpdate, d => d.Id != permissionBaseInfo.Id);

        if (_permissionRepository.Any(predicate))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.PERMISSION_CODE_EXIST, permissionBaseInfo.Code);
        }
        Permission permission;
        if (permissionBaseInfo.IsUpdate)
        {
            permission = await _permissionRepository.GetByIdAsync(permissionBaseInfo.Id);
            permission.Update(permissionBaseInfo.AppId, permissionBaseInfo.Name,
                permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
                permissionBaseInfo.Description, permissionBaseInfo.Order, permissionBaseInfo.Enabled);
            permission.SetParent(permissionBaseInfo.ParentId);
            permission.BindApiPermission(permissionBaseInfo.ApiPermissions.ToArray());
            await _permissionRepository.UpdateAsync(permission);
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
        permission = new Permission(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
            permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
            permissionBaseInfo.Description, permissionBaseInfo.Order, permissionBaseInfo.Enabled);
        permission.SetParent(permissionBaseInfo.ParentId);
        permission.BindApiPermission(permissionBaseInfo.ApiPermissions.ToArray());
        await _permissionRepository.AddAsync(permission);
        await _unitOfWork.SaveChangesAsync();
        upsertPermissionCommand.PermissionDetail.Id = permission.Id;
    }

    [EventHandler(1)]
    public async Task SeedPermissionsAsync(SeedPermissionsCommand seedPermissionsCommand)
    {
        var permissions = seedPermissionsCommand.Permissions.Where(p => !p.AffiliationPermissionRelations.Any() && !p.LeadingPermissionRelations.Any()).ToList();
        foreach (var permission in permissions)
        {
            await _permissionRepository.AddAsync(permission);
        }
        await _unitOfWork.SaveChangesAsync();
        permissions = seedPermissionsCommand.Permissions.Where(p => p.AffiliationPermissionRelations.Any() || p.LeadingPermissionRelations.Any()).ToList();
        foreach (var permission in permissions)
        {
            await _permissionRepository.AddAsync(permission);
        }
        await _unitOfWork.SaveChangesAsync();
    }
    #endregion
}