namespace Masa.Auth.Service.Admin.Application.Permissions;

public class CommandHandler
{
    readonly IRoleRepository _roleRepository;
    readonly IPermissionRepository _permissionRepository;

    public CommandHandler(IRoleRepository roleRepository, IPermissionRepository permissionRepository)
    {
        _roleRepository = roleRepository;
        _permissionRepository = permissionRepository;
    }

    [EventHandler]
    public async Task AddRoleAsync(AddRoleCommand command)
    {
        if (await _roleRepository.GetCountAsync(u => u.Name == command.Name) > 0)
            throw new UserFriendlyException($"Role with Name {command.Name} already exists");

        var role = new Role(command.Name, command.Description, command.Enabled);
        role.BindChildrenRoles(command.ChildrenRoles);
        role.BindPermissions(command.Permissions);
        await _roleRepository.AddAsync(role);
    }

    [EventHandler]
    public async Task UpdateUserAsync(UpdateRoleCommand command)
    {
        var role = await _roleRepository.FindAsync(u => u.Id == command.RoleId);
        if (role is null)
            throw new UserFriendlyException($"The current role does not exist");

        role.Update();
        role.BindChildrenRoles(command.ChildrenRoles);
        role.BindPermissions(command.Permissions);
        await _roleRepository.UpdateAsync(role);
    }

    [EventHandler]
    public async Task DeleteUserAsync(RemoveRoleCommand command)
    {
        var role = await _roleRepository.FindAsync(u => u.Id == command.RoleId);
        if (role is null)
            throw new UserFriendlyException($"The current role does not exist");

        //Todo
        //RemoveCheck
        await _roleRepository.RemoveAsync(role);
    }

    #region Permission

    [EventHandler]
    public async Task DeletePermissionAsync(RemovePermissionCommand removePermissionCommand)
    {
        var permission = await _permissionRepository.GetByIdAsync(removePermissionCommand.PermissionId);
        permission.DeleteCheck();
        await _permissionRepository.RemoveAsync(permission);
    }

    [EventHandler]
    public async Task CreatePermissionAsync(AddPermissionCommand addPermissionCommand)
    {
        var permissionBaseInfo = addPermissionCommand.PermissionDetail;
        if (permissionBaseInfo.IsUpdate)
        {
            var _permission = await _permissionRepository.GetByIdAsync(permissionBaseInfo.Id);
            _permission.Update(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
                permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
                permissionBaseInfo.Description, addPermissionCommand.Enabled);
            _permission.MoveParent(addPermissionCommand.ParentId);
            _permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
            await _permissionRepository.UpdateAsync(_permission);
            return;
        }
        var permission = new Permission(permissionBaseInfo.SystemId, permissionBaseInfo.AppId, permissionBaseInfo.Name,
            permissionBaseInfo.Code, permissionBaseInfo.Url, permissionBaseInfo.Icon, permissionBaseInfo.Type,
            permissionBaseInfo.Description, addPermissionCommand.Enabled);
        permission.MoveParent(addPermissionCommand.ParentId);
        permission.BindApiPermission(addPermissionCommand.ApiPermissions.ToArray());
        await _permissionRepository.AddAsync(permission);
    }

    #endregion

}