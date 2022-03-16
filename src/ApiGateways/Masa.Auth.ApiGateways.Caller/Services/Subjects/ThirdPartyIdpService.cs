namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyIdpService : ServiceBase
{

    List<ThirdPartyIdpItemResponse> ThirdPartyIdpItems = new List<ThirdPartyIdpItemResponse>()
    {
        new ThirdPartyIdpItemResponse(Guid.NewGuid(), "weixin", "weixin", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/weixin", "", default, DateTime.Now, null),
        new ThirdPartyIdpItemResponse(Guid.NewGuid(), "QQ", "qq", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/qq", "", default, DateTime.Now, null),
    };

    internal ThirdPartyIdpService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationItemsResponse<ThirdPartyIdpItemResponse>> GetThirdPartyIdpItemsAsync(GetThirdPartyIdpItemsRequest request)
    {
        var thirdPartyIdps = ThirdPartyIdpItems.Skip((request.PageIndex - 1) * request.PageSize).Take(request.PageSize);
        return await Task.FromResult(new PaginationItemsResponse<ThirdPartyIdpItemResponse>(ThirdPartyIdpItems.Count, 1, thirdPartyIdps));
    }

    public async Task<ThirdPartyIdpDetailResponse> GetThirdPartyIdpDetailAsync(Guid id)
    {
        return await Task.FromResult(ThirdPartyIdpDetailResponse.Default);
    }

    public async Task<List<ThirdPartyIdpItemResponse>> SelectThirdPartyIdpAsync()
    {
        return await Task.FromResult(ThirdPartyIdpItems);
    }

    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpRequest request)
    {
        ThirdPartyIdpItems.Add(new(Guid.NewGuid(), request.Name, request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.AuthenticationType, DateTime.Now, null));
        await Task.CompletedTask;
    }

    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpRequest request)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteThirdPartyIdpAsync(Guid id)
    {
        ThirdPartyIdpItems.Remove(ThirdPartyIdpItems.First(p => p.ThirdPartyIdpId == id));
        await Task.CompletedTask;
    }
}
