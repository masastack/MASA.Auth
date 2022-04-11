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

    public async Task RemoveAsync(Guid permissionId)
    {
        await DeleteAsync($"Delete?id={permissionId}");
    }
}
