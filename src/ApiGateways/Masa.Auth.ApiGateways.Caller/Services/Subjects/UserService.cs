namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    List<UserDto> Users = new List<UserDto>()
    {
        new UserDto(Guid.NewGuid(), "wuweilai", "wwl", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146736", "15168440402", "数闪科技", true, "15168440402", "824255785@qq.com", DateTime.Now.AddDays(-1)),
        new UserDto(Guid.NewGuid(), "wujiang", "wj", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146735", "15168440403", "数闪科技", false, "15168440403", "824255783@qq.com", DateTime.Now.AddDays(-2)),
    };

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
            ["enabled"] = request.Enabled.ToString(),
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

