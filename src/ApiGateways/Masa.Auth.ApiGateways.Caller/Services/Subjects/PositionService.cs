namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

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

    public async Task AddAsync(AddPositionDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdatePositionDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }
}

