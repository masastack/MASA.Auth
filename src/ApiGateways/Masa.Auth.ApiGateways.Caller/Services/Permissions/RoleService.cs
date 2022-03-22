namespace Masa.Auth.ApiGateways.Caller.Services.Permissions;

public class RoleService : ServiceBase
{
    List<RoleDto> Roles = new List<RoleDto>
    {
        new RoleDto(Guid.NewGuid(), "admin", "00001", 1, "admin Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
        new RoleDto(Guid.NewGuid(), "student", "10001", 1, "student Number One", true, DateTime.Now, DateTime.Now, "wwl", "wwl"),
    };

    internal RoleService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<RoleDto>> GetRolesAsync(GetRolesDto request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var roles = Roles.Skip(skip).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<RoleDto>(Roles.Count, 1, roles));
    }

    public async Task<RoleDetailDto> GetRoleDetailAsync(Guid id)
    {
        return await Task.FromResult(new RoleDetailDto());
    }

    public async Task<List<RoleSelectDto>> GetRoleSelectAsync()
    {
        return await Task.FromResult(Roles.Select(r => new RoleSelectDto(r.Id,r.Name)).ToList());
    }

    public async Task AddRoleAsync(AddRoleDto request)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateRoleAsync(UpdateRoleDto request)
    {
        await Task.CompletedTask;
    }
}

