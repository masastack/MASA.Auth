namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class PermissionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal PermissionService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/permission";
    }

    public async Task<List<SelectItemDto<PermissionTypes>>> GetTypesAsync()
    {
        return await GetAsync<List<SelectItemDto<PermissionTypes>>>($"GetTypes");
    }

    public async Task<List<AppPermissionDto>> GetApplicationPermissionsAsync(int systemId)
    {
        return await GetAsync<List<AppPermissionDto>>($"GetApplicationPermissions?systemId={systemId}");
    }

    public async Task<List<SelectItemDto<Guid>>> GetChildMenuPermissionsAsync(Guid permissionId)
    {
        return await GetAsync<List<SelectItemDto<Guid>>>($"GetChildMenuPermissions?permissionId={permissionId}");
    }

    public async Task<List<SelectItemDto<Guid>>> GetApiPermissionSelectAsync(string name)
    {
        return await GetAsync<List<SelectItemDto<Guid>>>($"GetApiPermissionSelect?name={name}");
    }

    public async Task RemoveAsync(Guid permissionId)
    {
        await DeleteAsync($"Delete?id={permissionId}");
    }
}
