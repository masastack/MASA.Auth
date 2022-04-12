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
    }

    [EventHandler]
    public async Task DeleteUserAsync(RemoveRoleCommand command)
    {
        var role = await _roleRepository.FindAsync(u => u.Id == command.Role.Id);
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
