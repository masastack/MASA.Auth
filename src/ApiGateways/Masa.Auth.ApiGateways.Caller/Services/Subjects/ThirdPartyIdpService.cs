namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class ThirdPartyIdpService : ServiceBase
{

    List<ThirdPartyIdpDto> ThirdPartyIdps = new List<ThirdPartyIdpDto>()
    {
        new ThirdPartyIdpDto(Guid.NewGuid(), "weixin", "weixin", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/weixin", "", default, DateTime.Now, null),
        new ThirdPartyIdpDto(Guid.NewGuid(), "QQ", "qq", Guid.NewGuid().ToString(), Guid.NewGuid().ToString(), "/qq", "", default, DateTime.Now, null),
    };

    internal ThirdPartyIdpService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<ThirdPartyIdpDto>> GetThirdPartyIdpsAsync(GetThirdPartyIdpIsDto request)
    {
        var thirdPartyIdps = ThirdPartyIdps.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<ThirdPartyIdpDto>(ThirdPartyIdps.Count, 1, thirdPartyIdps));
    }

    public async Task<ThirdPartyIdpDetailDto> GetThirdPartyIdpDetailAsync(Guid id)
    {
        return await Task.FromResult(ThirdPartyIdpDetailDto.Default);
    }

    public async Task<List<ThirdPartyIdpDto>> SelectThirdPartyIdpAsync()
    {
        return await Task.FromResult(ThirdPartyIdps);
    }

    public async Task AddThirdPartyIdpAsync(AddThirdPartyIdpDto request)
    {
        ThirdPartyIdps.Add(new(Guid.NewGuid(), request.Name, request.DisplayName, request.ClientId, request.ClientSecret, request.Url, request.Icon, request.AuthenticationType, DateTime.Now, null));
        await Task.CompletedTask;
    }

    public async Task UpdateThirdPartyIdpAsync(UpdateThirdPartyIdpDto request)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteThirdPartyIdpAsync(Guid id)
    {
        ThirdPartyIdps.Remove(ThirdPartyIdps.First(p => p.Id == id));
        await Task.CompletedTask;
    }
}
