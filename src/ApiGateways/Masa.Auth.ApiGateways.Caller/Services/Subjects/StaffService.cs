namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/staff/";
    }

    public async Task<PaginationDto<StaffDto>> GetListAsync(GetStaffsDto request)
    {
        var paramters = new Dictionary<string, string>
        {
            ["pageSize"] = request.PageSize.ToString(),
            ["page"] = request.Page.ToString(),
            ["search"] = request.Search.ToString(),
            ["enabled"] = request.Enabled?.ToString() ?? "",
        };
        return await SendAsync<PaginationDto<StaffDto>>(nameof(GetListAsync), paramters);
    }

    public async Task<StaffDetailDto> GetDetailAsync(Guid id)
    {
        var paramters = new Dictionary<string, string>
        {
            ["id"] = id.ToString(),
        };
        return await SendAsync<StaffDetailDto>(nameof(GetDetailAsync), paramters);
    }

    public async Task AddAsync(AddStaffDto request)
    {
        await SendAsync(nameof(AddAsync), request);
    }

    public async Task UpdateAsync(UpdateStaffDto request)
    {
        await SendAsync(nameof(UpdateAsync), request);
    }

    public async Task RemoveAsync(Guid id)
    {
        await SendAsync(nameof(RemoveAsync), new RemoveStaffDto(id));
    }
}

