namespace Masa.Auth.Service.Admin.Application.Permissions;

public class CommandHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;
    readonly RoleDomainService _roleDomainService;

    public CommandHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository, RoleDomainService roleDomainService)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
        _roleDomainService = roleDomainService;
    }

    [EventHandler]
    public async Task AddRoleAsync(AddRoleCommand command)
    {
        var roleDto = command.Role;
        if (await _roleRepository.GetCountAsync(u => u.Name == roleDto.Name) > 0)
            throw new UserFriendlyException($"Role with Name {roleDto.Name} already exists");

        var role = new Role(roleDto.Name, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.AddAsync(role);
    }

    [EventHandler]
    public async Task UpdateUserAsync(UpdateRoleCommand command)
    {
        var roleDto = command.Role;
        var role = await _roleRepository.GetByIdAsync(roleDto.Id);
        if (role is null)
            throw new UserFriendlyException($"The current role does not exist");

        role.Update(roleDto.Name, roleDto.Description, roleDto.Enabled, roleDto.Limit);
        role.BindChildrenRoles(roleDto.ChildrenRoles);
        role.BindPermissions(roleDto.Permissions);
        await _roleRepository.UpdateAsync(role);
        // update and check role limit
        var influenceRoles = new List<Guid>{ role.Id };
        await _roleDomainService.UpdateRoleLimitAsync(influenceRoles);
    }

    [EventHandler]
    public async Task RemoveRoleAsync(RemoveRoleCommand command)
    {
        var role = await _roleRepository.FindAsync(u => u.Id == command.Role.Id);
        if (role is null)
            throw new UserFriendlyException($"The current role does not exist");

        await _roleRepository.RemoveAsync(role);
    }

    #region Permission

    [EventHandler]
    public async Task ReomvePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        var permission = await _permissionRepository.GetByIdAsync(removePermissionCommand.PermissionId);
        permission.DeleteCheck();
        await _permissionRepository.RemoveAsync(permission);
    }

    [EventHandler]
    public async Task AddPermissionAsync(AddPermissionCommand addPermissionCommand)
    {
        var permissionBaseInfo = addPermissionCommand.PermissionDetail;
        if (permissionBaseInfo.IsUpdate)
        {
            var _permission = await _permissionRepository.GetByIdAsync(permissionBaseInfo.Id);
            _permission.Update(permissionBaseInfo.AppId, permissionBaseInfo.Name,
                permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
                permissionBaseInfo.Description, addPermissionCommand.Enabled);
            _permission.SetParent(addPermissionCommand.ParentId);
            _permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
            await _permissionRepository.UpdateAsync(_permission);
            return;
        }
        var permission = new Permission(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
            permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
            permissionBaseInfo.Description, addPermissionCommand.Enabled);
        permission.SetParent(addPermissionCommand.ParentId);
        permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
        await _permissionRepository.AddAsync(permission);
    }

    #endregion

}