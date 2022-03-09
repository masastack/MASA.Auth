namespace Masa.Auth.Sdk;

public class AuthClient
{
    public AuthClient()
    {

    }

    #region User

    List<UserItemResponse> UserItems = new List<UserItemResponse>()
    {
        new UserItemResponse(Guid.NewGuid(),"wuweilai","wwl","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146736","15168440402","数闪科技",true,"15168440402","824255785@qq.com","","","","","","","",""),
        new UserItemResponse(Guid.NewGuid(),"wujiang","wj","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146735","15168440403","数闪科技",false,"15168440403","824255783@qq.com","","","","","","","",""),
    };

    public async Task<ApiResultResponse<List<UserItemResponse>>> GetUserItemsAsync(GetUserItemsRequest request)
    {
        var users = UserItems.Where(u =>u.Enabled==request.Enabled && (u.Name.Contains(request.Search) || u.DisplayName.Contains(request.Search) || u.PhoneNumber.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<UserItemResponse>>.ResponseSuccess(users, "查询成功"));
    }

    public async Task<ApiResultResponse<UserItemResponse>> GetUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<UserItemResponse>.ResponseSuccess(UserItems.First(u => u.UserId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddUserAsync(AddUserRequest request)
    {
        UserItems.Add(new UserItemResponse(Guid.NewGuid(), request.Name, request.DisplayName, request.Avatar, request.IDCard, request.PhoneNumber,request.CompanyName, request.Enabled, request.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditUserAsync(EditUserRequest request)
    {
        var oldData = UserItems.First(u => request.UserId == request.UserId);
        UserItems.Remove(oldData);
        UserItems.Add(new UserItemResponse(request.UserId, request.Name, request.DisplayName, request.Avatar, oldData.IDCard, oldData.PhoneNumber, request.CompanyName, request.Enabled, oldData.PhoneNumber, request.Email, request.HouseholdRegisterAddress, request.HouseholdRegisterProvinceCode, request.HouseholdRegisterCityCode, request.HouseholdRegisterDistrictCode, request.ResidentialAddress, request.ResidentialProvinceCode, request.ResidentialCityCode, request.ResidentialDistrictCode));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    #endregion

    #region Platform

    List<PlatformItemResponse> PlatformItems = new List<PlatformItemResponse>()
    {
        new PlatformItemResponse(Guid.NewGuid(),"微信","weixin",Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),"/weixin","",VerifyType.OAuth,DateTime.Now,null),
        new PlatformItemResponse(Guid.NewGuid(),"QQ","qq",Guid.NewGuid().ToString(),Guid.NewGuid().ToString(),"/qq","",VerifyType.OAuth,DateTime.Now,null),
    };

    public async Task<ApiResultResponse<List<PlatformItemResponse>>> GetPlatformItemsAsync(GetPlatformItemsRequest request)
    {
        return await Task.FromResult(ApiResultResponse<List<PlatformItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    public async Task<ApiResultResponse<PlatformItemResponse>> GetPlatformDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<PlatformItemResponse>.ResponseSuccess(PlatformItems.First(p => p.PlatformId == id), "查询成功"));
    }

    public async Task<ApiResultResponse<List<PlatformItemResponse>>> SelectPlatformAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<PlatformItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    #endregion

    #region ThirdPartyUser

    List<ThirdPartyUserItemResponse> ThirdPartyUserItems => new List<ThirdPartyUserItemResponse>()
    {
        new ThirdPartyUserItemResponse(Guid.Parse("A446CD5D-B35F-7029-4A30-8232744A3A8E"),PlatformItems[0].PlatformId,true,UserItems[0]),
        new ThirdPartyUserItemResponse(Guid.Parse("8056549B-7D96-E377-2D03-A27C77837EFB"),PlatformItems[1].PlatformId,false,UserItems[1]),
    };

    public async Task<ApiResultResponse<List<ThirdPartyUserItemResponse>>> GetThirdPartyUserItemsAsync(GetUserItemsRequest request)
    {
        var thirdPartyUsers = ThirdPartyUserItems.Where(tpu => tpu.Enabled == request.Enabled && (tpu.User.Name.Contains(request.Search) || tpu.User.PhoneNumber.Contains(request.Search) || tpu.User.DisplayName.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyUserItemResponse>>.ResponseSuccess(thirdPartyUsers, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyUserItemResponse>> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyUserItemResponse>.ResponseSuccess(ThirdPartyUserItems.First(u => u.ThirdPartyPlatformId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyUserAsync(AddThirdPartyUserRequest request)
    {
        await AddUserAsync(request.User);
        ThirdPartyUserItems.Add(new ThirdPartyUserItemResponse(Guid.NewGuid(), request.ThirdPartyPlatformId,request.Enabled, UserItems.First(u => u.PhoneNumber == request.User.PhoneNumber)));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditThirdPartyUserAsync(EditThirdPartyUserRequest request)
    {
        await EditUserAsync(request.User);
        var oldData = ThirdPartyUserItems.First(tpu => tpu.ThirdPartyUserId == request.ThirdPartyUserId);
        ThirdPartyUserItems.Remove(oldData);
        ThirdPartyUserItems.Add(new ThirdPartyUserItemResponse(request.ThirdPartyUserId, oldData.ThirdPartyPlatformId, request.Enabled, UserItems.First(u => u.UserId==request.User.UserId)));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    #endregion
}

