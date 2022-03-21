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
        var roles = Roles.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
        return await Task.FromResult(new PaginationDto<RoleDto>(Roles.Count, 1, Roles));
    }

    public async Task<RoleDetailDto> GetRoleDetailAsync(Guid id)
    {
        return await Task.FromResult(RoleDetailDto.Default);
    }

    public async Task<List<RoleDto>> SelectRolesAsync()
    {
        return await Task.FromResult(Roles);
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

