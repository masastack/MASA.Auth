namespace Masa.Contrib.BasicAbilities.Auth.Callers.Subjects;

public class UserCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    List<UserItemResponse> UserItems = new List<UserItemResponse>()
    {
        new UserItemResponse(Guid.NewGuid(), "wuweilai", "wwl", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146736", "15168440402", "数闪科技", true, "15168440402", "824255785@qq.com", "", "", "", "", "", "", "", "", DateTime.Now.AddDays(-1)),
        new UserItemResponse(Guid.NewGuid(), "wujiang", "wj", "https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg", "362330199508146735", "15168440403", "数闪科技", false, "15168440403", "824255783@qq.com", "", "", "", "", "", "", "", "", DateTime.Now.AddDays(-2)),
    };

    internal UserCaller(IServiceProvider serviceProvider, Options options) : base(serviceProvider)
    {
        Name = nameof(ThirdPartyIdpCaller);
        BaseAddress = options.AuthServiceBaseAdress;
    }

    public async Task<ApiResultResponse<List<UserItemResponse>>> GetUserItemsAsync(GetUserItemsRequest request)
    {
        var users = UserItems.Where(u => u.Enabled == request.Enabled && (u.Name.Contains(request.Search) || u.DisplayName.Contains(request.Search) || u.PhoneNumber.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<UserItemResponse>>.ResponseSuccess(users, "查询成功"));
    }

    public async Task<ApiResultResponse<UserItemResponse>> GetUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<UserItemResponse>.ResponseSuccess(UserItems.First(u => u.UserId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddUserAsync(AddUserRequest request)
    {
        UserItems.Add(new UserItemResponse(Guid.NewGuid(), request.Name, request.DisplayName, request.Avatar, request.IDCard, request.PhoneNumber, request.CompanyName, request.Enabled, request.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode, DateTime.Now));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditUserAsync(EditUserRequest request)
    {
        var oldData = UserItems.First(u => request.UserId == request.UserId);
        UserItems.Remove(oldData);
        UserItems.Add(new UserItemResponse(request.UserId, request.Name, request.DisplayName, request.Avatar, oldData.IDCard, oldData.PhoneNumber, request.CompanyName, request.Enabled, oldData.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode, oldData.CreationTime));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> DeleteUserAsync(Guid userId)
    {
        UserItems.Remove(UserItems.First(u => u.UserId == userId));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("删除成功"));
    }
}

