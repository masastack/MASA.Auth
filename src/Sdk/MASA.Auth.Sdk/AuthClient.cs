namespace Masa.Auth.Sdk;

public class AuthClient
{
    public AuthClient()
    {

    }

    #region ThirdPartyUser

    List<ThirdPartyUserItemResponse> ThirdPartyUserItems = new List<ThirdPartyUserItemResponse>()
    {
        new ThirdPartyUserItemResponse(Guid.NewGuid(),Guid.NewGuid(),new UserItemResponse(Guid.NewGuid(),"wuweilai","wwl","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146736","15168440402","123","数闪科技","15168440402","824255785@qq.com","","","","","","","",""),UserStates.Enabled),
        new ThirdPartyUserItemResponse(Guid.NewGuid(),Guid.NewGuid(),new UserItemResponse(Guid.NewGuid(),"wujiang","wj","https://masa-blazor-pro.lonsid.cn/img/avatar/2.svg","362330199508146735","15168440403","123","数闪科技","15168440403","824255783@qq.com","","","","","","","",""),UserStates.Enabled),
    };

    public async Task<ApiResultResponse<List<ThirdPartyUserItemResponse>>> GetThirdPartyUserItemsAsync(GetThirdPartyUserItemsRequest request)
    {
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyUserItemResponse>>.ResponseSuccess(ThirdPartyUserItems, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyUserItemResponse>> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyUserItemResponse>.ResponseSuccess(ThirdPartyUserItems.First(u => u.ThirdPartyPlatformId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyUserAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyUserItemResponse>.ResponseSuccess(""));
    }

    #endregion
}

