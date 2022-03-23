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

    [EventHandler]
    public async Task DeletePermissionAsync(RemovePermissionCommand deletePermissionCommand)
    {
        var permission = await _permissionRepository.GetByIdAsync(deletePermissionCommand.PermissionId);
        permission.DeleteCheck();
        await _permissionRepository.RemoveAsync(permission);
    }

    [EventHandler]
    public async Task CreatePermissionAsync(AddPermissionCommand createPermissionCommand)
    {
        var permission = new Permission(createPermissionCommand.SystemId, createPermissionCommand.AppId, createPermissionCommand.Name,
            createPermissionCommand.Icon, createPermissionCommand.Url, createPermissionCommand.Icon, createPermissionCommand.Type,
            createPermissionCommand.Description);
        permission.SetEnabled(createPermissionCommand.Enabled);
        permission.MoveParent(createPermissionCommand.ParentId);
        permission.BindApiPermission(createPermissionCommand.ApiPermissionIds.ToArray());
        await _permissionRepository.AddAsync(permission);
    }
}
