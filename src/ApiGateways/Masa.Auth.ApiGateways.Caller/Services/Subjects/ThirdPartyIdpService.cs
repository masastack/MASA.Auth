namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyIdpService : ServiceBase
{

    List<ThirdPartyIdpIDto> ThirdPartyIdpItems = new List<ThirdPartyIdpIDto>()
    {
        new ThirdPartyIdpIDto(Guid.NewGuid(), "weixin", "weixin", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/weixin", "", default, DateTime.Now, null),
        new ThirdPartyIdpIDto(Guid.NewGuid(), "QQ", "qq", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/qq", "", default, DateTime.Now, null),
    };

    internal ThirdPartyIdpService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<ThirdPartyIdpIDto>> GetThirdPartyIdpItemsAsync(GetThirdPartyIdpIsDto request)
    {
        var thirdPartyIdps = ThirdPartyIdpItems.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<ThirdPartyIdpIDto>(ThirdPartyIdpItems.Count, 1, thirdPartyIdps));
    }

    public async Task<ThirdPartyIdpDetailDto> GetThirdPartyIdpDetailAsync(Guid id)
    {
        return await Task.FromResult(ThirdPartyIdpDetailDto.Default);
    }

    public async Task<List<ThirdPartyIdpIDto>> SelectThirdPartyIdpAsync()
    {
        return await Task.FromResult(ThirdPartyIdpItems);
    }

    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpDto request)
    {
        ThirdPartyIdpItems.Add(new(Guid.NewGuid(), request.Name, request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.AuthenticationType, DateTime.Now, null));
        await Task.CompletedTask;
    }

    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpDto request)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteThirdPartyIdpAsync(Guid id)
    {
        ThirdPartyIdpItems.Remove(ThirdPartyIdpItems.First(p => p.ThirdPartyIdpId == id));
        await Task.CompletedTask;
    }
}
