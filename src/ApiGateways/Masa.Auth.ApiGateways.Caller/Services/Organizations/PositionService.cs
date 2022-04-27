namespace Masa.Auth.ApiGateways.Caller.Services.Organizations;

public class PositionService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal PositionService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/position/";
    }

    public async Task<PositionDetailDto> GetDetailAsync(Guid id)
    {
        return await SendAsync<object, PositionDetailDto>(nameof(GetDetailAsync), new { id });
    }

    public async Task<List<PositionSelectDto>> GetSelectAsync(string name = "")
    {
        return await SendAsync<object, List<PositionSelectDto>>(nameof(GetSelectAsync), new { name });
    }

    public async Task AddOrUpdateAsync(AddOrUpdatePositionDto request)
    {
        await SendAsync(nameof(AddOrUpdateAsync), request);
    }
}

