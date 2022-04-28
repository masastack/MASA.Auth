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
        return await SendAsync<GetRolesDto, PaginationDto<RoleDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<RoleSelectDto>> GetTopRoleSelectAsync(Guid roleId)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetTopRoleSelectAsync), new { roleId });
    }

    public async Task<List<RoleSelectDto>> GetSelectForUserAsync(Guid userId = default)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForUserAsync), new { userId });
    }


    public async Task<List<RoleSelectDto>> GetSelectForRoleAsync(Guid roleId = default)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForRoleAsync), new { roleId });
    }

    public async Task<List<RoleSelectDto>> GetSelectForTeamAsync(Guid teamId = default, TeamMemberTypes teamMemberType = TeamMemberTypes.Member)
    {
        return await SendAsync<object, List<RoleSelectDto>>(nameof(GetSelectForTeamAsync), new { teamId, teamMemberType });
    }

    public async Task<RoleDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, RoleDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task<RoleOwnerDto> GetRoleOwnerAsync(Guid id)
    {
        return await SendAsync<object, RoleOwnerDto>(nameof(GetRoleOwnerAsync), new { id });
    }

    public async Task<List<Guid>> GetPermissionsByRoleAsync(List<Guid> roles)
    {
        return await SendAsync<object, List<Guid>>(nameof(GetPermissionsByRoleAsync), new { ids = string.Join(',', roles) });
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

