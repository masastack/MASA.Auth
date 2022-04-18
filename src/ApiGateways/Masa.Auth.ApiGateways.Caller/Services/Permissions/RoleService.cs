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

    public async Task<List<RoleSelectDto>> GetTopRoleSelectAsync(Guid roleId)
    {
        var paramters = new Dictionary<string, string>
        {
            ["roleId"] = roleId.ToString(),
        };
        return await SendAsync<List<RoleSelectDto>>(nameof(GetTopRoleSelectAsync), paramters);
    }

    public async Task<List<RoleSelectDto>> GetSelectForUserAsync(Guid userId = default)
    {
        var paramters = new Dictionary<string, string>
        {
            ["userId"] = userId.ToString(),
        };
        return await SendAsync<List<RoleSelectDto>>(nameof(GetSelectForUserAsync), paramters);
    }


    public async Task<List<RoleSelectDto>> GetSelectForRoleAsync(Guid roleId = default)
    {
        var paramters = new Dictionary<string, string>
        {
            ["roleId"] = roleId.ToString(),
        };
        return await SendAsync<List<RoleSelectDto>>(nameof(GetSelectForRoleAsync), paramters);
    }

    public async Task<List<RoleSelectDto>> GetSelectForTeamAsync(Guid teamId = default, TeamMemberTypes teamMemberType = TeamMemberTypes.Member)
    {
        var paramters = new Dictionary<string, string>
        {
            ["teamId"] = teamId.ToString(),
            ["teamMemberType"] = teamMemberType.ToString(),
        };
        return await SendAsync<List<RoleSelectDto>>(nameof(GetSelectForTeamAsync), paramters);
    }

    public async Task<RoleDetailDto> GetDetailAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };

        return await SendAsync<RoleDetailDto>(nameof(GetDetailAsync), paramters);
    }

    public async Task<RoleOwnerDto> GetRoleOwnerAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };

        return await SendAsync<RoleOwnerDto>(nameof(GetRoleOwnerAsync), paramters);
    }

    public async Task<List<Guid>> GetPermissionsByRoleAsync(List<Guid> roles)
    {
        var paramters = new Dictionary<string, string>
        {
            ["ids"] = string.Join(',', roles),
        };

        return await SendAsync<List<Guid>>(nameof(GetPermissionsByRoleAsync), paramters);
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

