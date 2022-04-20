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
        return await SendAsync<GetStaffsDto, PaginationDto<StaffDto>>(nameof(GetListAsync), request);
    }

    public async Task<List<StaffSelectDto>> GetSelectAsync(string name)
    {
        var paramters = new Dictionary<string, string>
        {
            ["name"] = name,
        };
        return await SendAsync<List<StaffSelectDto>>(nameof(GetSelectAsync), paramters);
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

