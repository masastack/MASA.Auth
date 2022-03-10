namespace Masa.Auth.Service.Application.Permissions;

public class CommandHandler
{
    readonly IPermissionRepository _permissionRepository;

    public CommandHandler(IPermissionRepository permissionRepository)
    {
        _permissionRepository = permissionRepository;
    }

    public async Task DeletePermissionAsync(DeletePermissionCommand deletePermissionCommand)
    {
        var permission = await _permissionRepository.GetByIdAsync(deletePermissionCommand.PermissionId);
        permission.DeleteCheck();
        await _permissionRepository.RemoveAsync(permission);
    }

    public async Task CreatePermissionAsync(CreatePermissionCommand createPermissionCommand)
    {

    }
}
