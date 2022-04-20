namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal UserService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/user/";
    }

    public async Task<PaginationDto<UserDto>> GetListAsync(GetUsersDto request)
    {
        return await SendAsync<GetUsersDto, PaginationDto<UserDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<UserSelectDto>> GetSelectAsync(string search)
    {
        var paramters = new Dictionary<string, string>
        {
            ["search"] = search
        };

        return await SendAsync<List<UserSelectDto>>(nameof(GetSelectAsync), paramters);
    }

    public async Task<UserDetailDto> GetDetailAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };
        return await SendAsync<UserDetailDto>(nameof(GetDetailAsync), paramters);
    }

    public async Task AddAsync(AddUserDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateUserDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task UpdateUserAuthorizationAsync(UpdateUserAuthorizationDto request)
    {
        await SendAsync(nameof(UpdateUserAuthorizationAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveUserDto(id));
    }
}

