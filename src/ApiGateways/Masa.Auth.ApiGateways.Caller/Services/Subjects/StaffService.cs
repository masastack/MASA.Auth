namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    protected override string BaseUrl { get; set; }

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/staff/";
    }

    public async Task<PaginationDto<StaffDto>> GetStaffsAsync(GetStaffsDto request)
    {
        var paramters = new Dictionary<string, string>
        {
            ["pageSize"] = request.PageSize.ToString(),
            ["page"] = request.Page.ToString(),
            ["search"] = request.Search ?? "",
            ["enabled"] = request.Enabled?.ToString() ?? "",
            ["departmentId"] = request.DepartmentId.ToString()
        };
        return await GetAsync<PaginationDto<StaffDto>>(nameof(GetStaffsAsync), paramters);
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
        return await GetAsync<StaffDetailDto>(nameof(GetStaffDetailAsync), paramters);
    }

    public async Task AddStaffAsync(AddStaffDto request)
    {
        await PutAsync(nameof(AddStaffAsync), request);
    }

    public async Task UpdateStaffAsync(UpdateStaffDto request)
    {
        await PostAsync(nameof(UpdateStaffAsync), request);
    }

    public async Task RemoveStaffAsync(Guid id)
    {
        await DeleteAsync(nameof(RemoveStaffAsync), new RemoveStaffDto(id));
    }
}

