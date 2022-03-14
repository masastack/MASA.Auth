namespace Masa.Contrib.BasicAbilities.Auth.Callers.Subjects;

internal class ThirdPartyIdpCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    List<ThirdPartyIdpItemResponse> PlatformItems = new List<ThirdPartyIdpItemResponse>()
    {
        new ThirdPartyIdpItemResponse(Guid.NewGuid(),  "weixin", "weixin", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/weixin", "", VerifyType.OAuth, DateTime.Now, null),
        new ThirdPartyIdpItemResponse(Guid.NewGuid(), "QQ", "qq", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/qq", "", VerifyType.OAuth, DateTime.Now, null),
    };

    internal ThirdPartyIdpCaller(IServiceProvider serviceProvider, Options options) : base(serviceProvider)
    {
        Name = nameof(ThirdPartyIdpCaller);
        BaseAddress = options.AuthServiceBaseAdress;
    }

    public async Task<ApiResultResponse<List<ThirdPartyIdpItemResponse>>> GetThirdPartyPlatformItemsAsync(GetThirdPartyIdpItemsRequest request)
    {
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyIdpItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyIdpItemResponse>> GetThirdPartyPlatformDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyIdpItemResponse>.ResponseSuccess(PlatformItems.First(p => p.ThirdPartyPlatformId == id), "查询成功"));
    }

    public async Task<ApiResultResponse<List<ThirdPartyIdpItemResponse>>> SelectThirdPartyPlatformAsync()
    {
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyIdpItemResponse>>.ResponseSuccess(PlatformItems, "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyPlatformAsync(AddThirdPartyIdpRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }

    public async Task<ApiResultResponse> EditThirdPartyPlatformAsync(EditThirdPartyIdpRequest request)
    {
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("编辑成功"));
    }

    public async Task<ApiResultResponse> DeleteThirdPartyPlatformAsync(Guid id)
    {
        PlatformItems.Remove(PlatformItems.First(p => p.ThirdPartyPlatformId == id));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("删除成功"));
    }
}
