namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/user/";
    }

    public async Task<PaginationDto<UserDto>> GetUsersAsync(GetUsersDto request)
    {
        var paramters = new Dictionary<string, string>
        {
            ["pageSize"] = request.PageSize.ToString(),
            ["page"] = request.Page.ToString(),
            ["userId"] = request.UserId.ToString(),
            ["enabled"] = request.Enabled?.ToString() ?? "",
        };
        return await GetAsync<PaginationDto<UserDto>>(nameof(GetUsersAsync), paramters);
    }

    public async Task<UserDetailDto> GetUserDetailAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };
        return await GetAsync<UserDetailDto>(nameof(GetUserDetailAsync), paramters);
    }

    public async Task AddUserAsync(AddUserDto request)
    {
        await PutAsync(nameof(AddUserAsync), request);
    }

    public async Task UpdateUserAsync(UpdateUserDto request)
    {
        await PostAsync(nameof(UpdateUserAsync), request);
    }

    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationDto request)
    {
        await PostAsync(nameof(UpdateUserAuthorizationAsync), request);
    }

    public async Task RemoveUserAsync(Guid id)
    {
        await DeleteAsync(nameof(RemoveUserAsync), new RemoveUserDto(id));
    }
}

