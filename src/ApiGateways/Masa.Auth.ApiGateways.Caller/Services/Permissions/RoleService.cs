namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class RoleService : ServiceBase
{
    List<RoleDto> Roles = new List<RoleDto>
    {
        new RoleDto(Guid.NewGuid(), "admin", "00001", 1, "admin Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
        new RoleDto(Guid.NewGuid(), "student", "10001", 1, "student Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
    };

    protected override string BaseUrl { get; set; }

    internal RoleService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/role/";
    }

    public async Task<PaginationDto<RoleDto>> GetListAsync(GetRolesDto request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var roles = Roles.Skip(skip).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<RoleDto>(Roles.Count, 1, roles));
    }

    public async Task<RoleDetailDto> GetDetailAsync(Guid id)
    {
        return await Task.FromResult(new RoleDetailDto());
    }

    public async Task<List<RoleSelectDto>> GetSelectAsync()
    {
        return await Task.FromResult(Roles.Select(r => new RoleSelectDto(r.Id, r.Name)).ToList());
    }

    public async Task AddAsync(AddRoleDto request)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(UpdateRoleDto request)
    {
        await Task.CompletedTask;
    }

    public async Task RemoveAsync(Guid id)
    {
        await Task.CompletedTask;
    }
}

