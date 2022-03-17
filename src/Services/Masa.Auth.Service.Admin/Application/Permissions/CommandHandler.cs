namespace Masa.Auth.Service.Admin.Application.Permissions;

public class CommandHandler
{
    readonly IPermissionRepository _permissionRepository;

    public CommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
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
