namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyUserService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal ThirdPartyUserService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/thirdPartyUser/";
    }

    public async Task<PaginationDto<ThirdPartyUserDto>> GetThirdPartyUsersAsync(GetThirdPartyUsersDto request)
    {
        return await SendAsync<GetThirdPartyUsersDto, PaginationDto<ThirdPartyUserDto>>(nameof(GetThirdPartyUsersAsync), request);
    }

    public async Task<ThirdPartyUserDetailDto> GetThirdPartyUserDetailAsync(Guid id)
    {
        return await SendAsync<object, ThirdPartyUserDetailDto>(nameof(GetThirdPartyUserDetailAsync), new { id });
    }
}

