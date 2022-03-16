namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyUserService : ServiceBase
{
    internal ThirdPartyUserService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<PaginationItemsResponse<ThirdPartyUserItemResponse>> GetThirdPartyUserItemsAsync(GetThirdPartyUserItemsRequest request)
    {
        return await Task.FromResult(new PaginationItemsResponse<ThirdPartyUserItemResponse>(0, 0, new List<ThirdPartyUserItemResponse>()));
    }

    public async Task<ThirdPartyUserDetailResponse> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await Task.FromResult(ThirdPartyUserDetailResponse.Default);
    }

    public async Task AddThirdPartyUserAsync(AddThirdPartyUserRequest request)
    {
        await Task.CompletedTask;
    }
}

