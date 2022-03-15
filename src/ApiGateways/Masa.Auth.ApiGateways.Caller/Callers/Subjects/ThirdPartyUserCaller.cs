using Masa.Auth.ApiGateways.Caller.Request.Subjects;
using Masa.Auth.ApiGateways.Caller.Response;
using Masa.Auth.ApiGateways.Caller.Response.Subjects;

namespace Masa.Auth.ApiGateways.Caller.Callers.Subjects;

public class ThirdPartyUserCaller : CallerBase
{
    protected override string BaseAddress { get; set; }

    public override string Name { get; set; }

    List<ThirdPartyUserItemResponse> ThirdPartyUserItems => new List<ThirdPartyUserItemResponse>()
    {
        //new ThirdPartyUserItemResponse(Guid.Parse("A446CD5D-B35F-7029-4A30-8232744A3A8E"),PlatformItems[0].ThirdPartyPlatformId,true,UserItems[0],DateTime.Now,DateTime.Now,Guid.Empty),
        //new ThirdPartyUserItemResponse(Guid.Parse("8056549B-7D96-E377-2D03-A27C77837EFB"),PlatformItems[1].ThirdPartyPlatformId,false,UserItems[1],DateTime.Now,DateTime.Now,Guid.Empty),
    };

    internal ThirdPartyUserCaller(IServiceProvider serviceProvider, Options options) : base(serviceProvider)
    {
        Name = nameof(ThirdPartyIdpCaller);
        BaseAddress = options.AuthServiceBaseAdress;
    }

    public async Task<ApiResultResponse<List<ThirdPartyUserItemResponse>>> GetThirdPartyUserItemsAsync(GetThirdPartyUserItemsRequest request)
    {
        var thirdPartyUsers = ThirdPartyUserItems.Where(tpu => tpu.Enabled == request.Enabled && tpu.ThirdPartyIdpId == request.ThirdPartyPlatformId && (tpu.User.Name.Contains(request.Search) || tpu.User.PhoneNumber.Contains(request.Search) || tpu.User.DisplayName.Contains(request.Search))).Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(ApiResultResponse<List<ThirdPartyUserItemResponse>>.ResponseSuccess(thirdPartyUsers, "查询成功"));
    }

    public async Task<ApiResultResponse<ThirdPartyUserItemResponse>> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ApiResultResponse<ThirdPartyUserItemResponse>.ResponseSuccess(ThirdPartyUserItems.First(u => u.ThirdPartyIdpId == id), "查询成功"));
    }

    public async Task<ApiResultResponse> AddThirdPartyUserAsync(AddThirdPartyUserRequest request)
    {
        //await AddUserAsync(request.User);
        //ThirdPartyUserItems.Add(new ThirdPartyUserItemResponse(Guid.NewGuid(), request.ThirdPartyIdpId, request.Enabled, UserItems.First(u => u.PhoneNumber == request.User.PhoneNumber), DateTime.Now, DateTime.Now, Guid.Empty));
        return await Task.FromResult(ApiResultResponse.ResponseSuccess("新增成功"));
    }
}

