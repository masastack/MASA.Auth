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
            ["search"] = request.Search ?? "",
            ["enabled"] = request.Enabled?.ToString() ?? "",
            ["departmentId"] = request.DepartmentId.ToString()
        };
        return await SendAsync<PaginationDto<StaffDto>>(nameof(GetListAsync), paramters);
    }

    public async Task<List<StaffSelectDto>> GetStaffSelectAsync(string name)
    {
        var paramters = new Dictionary<string, string>
        {
            ["name"] = name,
        };
        return await GetAsync<List<StaffSelectDto>>(nameof(GetStaffSelectAsync), paramters);
    }

    public async Task<StaffDetailDto> GetStaffDetailAsync(Guid id)
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

