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
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };
        return await SendAsync<PositionDetailDto>(nameof(GetDetailAsync), paramters);
    }

    public async Task<List<PositionSelectDto>> GetSelectAsync(string name = "")
    {
        var paramters = new Dictionary<string, string>
        {
            ["name"] = name,
        };
        return await SendAsync<List<PositionSelectDto>>(nameof(GetSelectAsync), paramters);
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

