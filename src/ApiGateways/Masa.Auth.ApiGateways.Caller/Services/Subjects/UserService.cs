namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class UserService : ServiceBase
{
    List<UserDto> UserItems = new List<UserDto>()
    {
        new UserDto(Guid.NewGuid(), "wuweilai", "wwl", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146736", "15168440402", "数闪科技", true, "15168440402", "824255785@qq.com", DateTime.Now.AddDays(-1)),
        new UserDto(Guid.NewGuid(), "wujiang", "wj", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146735", "15168440403", "数闪科技", false, "15168440403", "824255783@qq.com", DateTime.Now.AddDays(-2)),
    };

    internal UserService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<UserDto>> GetUserItemsAsync(GetUsersDto request)
    {
        var users = UserItems.Where(u => u.Enabled == request.Enabled).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<UserDto>(UserItems.Count, 1, users));
    }

    public async Task<UserDetailDto> GetUserDetailAsync(Guid id)
    {
        return await Task.FromResult(UserDetailDto.Default);
    }

    public async Task AddUserAsync(AddUserDto request)
    {
        UserItems.Add(new UserDto(Guid.NewGuid(), request.Name, request.DisplayName, request.Avatar, request.IDCard, request.PhoneNumber, request.CompanyName, request.Enabled, request.PhoneNumber, request.Email, DateTime.Now));
        await Task.CompletedTask;
    }

    public async Task UpdateUserAsync(UpdateUserDto request)
    {
        var oldData = UserItems.First(u => request.UserId == request.UserId);
        UserItems.Remove(oldData);
        UserItems.Add(new UserDto(request.UserId, request.Name, request.DisplayName, request.Avatar, oldData.IDCard, oldData.PhoneNumber, request.CompanyName, request.Enabled, oldData.PhoneNumber, oldData.Email, oldData.CreationTime));
        await Task.CompletedTask;
    }

    public async Task DeleteUserAsync(Guid userId)
    {
        UserItems.Remove(UserItems.First(u => u.UserId == userId));
        await Task.CompletedTask;
    }
}

