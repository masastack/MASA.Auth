namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class RoleService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal RoleService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/role/";
    }

    public async Task<PaginationDto<RoleDto>> GetListAsync(GetRolesDto request)
    {
        var paramters = new Dictionary<string, string>
        {
            ["pageSize"] = request.PageSize.ToString(),
            ["page"] = request.Page.ToString(),
            ["search"] = request.Search,
            ["enabled"] = request.Enabled?.ToString() ?? "",
        };

        return await SendAsync<PaginationDto<RoleDto>>(nameof(GetListAsync), paramters);
    }

    public async Task<List<RoleSelectDto>> GetSelectAsync()
    {
        return await SendAsync<List<RoleSelectDto>>(nameof(GetSelectAsync), query: null);
    }

    public async Task<RoleDetailDto> GetDetailAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };

        return await SendAsync<RoleDetailDto>(nameof(GetDetailAsync), paramters);
    }

    public async Task AddAsync(AddRoleDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateRoleDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveRoleDto(id));
    }
}

