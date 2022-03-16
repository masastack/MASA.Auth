namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class RoleService : ServiceBase
{
    List<RoleItemResponse> Roles = new List<RoleItemResponse>
    {
        new RoleItemResponse(Guid.NewGuid(), "admin", "00001", 1, "admin Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
        new RoleItemResponse(Guid.NewGuid(), "student", "10001", 1, "student Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
    };

    internal RoleService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationItemsResponse<RoleItemResponse>> GetRoleItemsAsync(GetRoleItemsRequest request)
    {
        var roles = Roles.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
        return await Task.FromResult(new PaginationItemsResponse<RoleItemResponse>(Roles.Count, 1, Roles));
    }

    public async Task<RoleDetailResponse> GetRoleDetailAsync(Guid id)
    {
        return await Task.FromResult(RoleDetailResponse.Default);
    }

    public async Task<List<RoleItemResponse>> SelectRolesAsync()
    {
        return await Task.FromResult(Roles);
    }

    public async Task AddRoleAsync(AddRoleRequest request)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateRoleAsync(UpdateRoleRequest request)
    {
        await Task.CompletedTask;
    }
}

