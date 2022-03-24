namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    List<StaffDto> Staffs => new List<StaffDto>()
    {

    };

    protected override string BaseUrl { get; set; }

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {
        BaseUrl = "api/staff/";
    }



    public async Task<PaginationDto<StaffDto>> GetStaffsAsync(GetStaffsDto request)
    {
        var skip = (request.Page - 1) * request.PageSize;
        var staffs = Staffs.Skip(skip).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<StaffDto>(Staffs.Count, 1, staffs));

        //var paramters = new Dictionary<string, string>
        //{
        //    ["staffId"] = request.StaffId.ToString(),
        //    ["enabled"] = request.Enabled.ToString(),
        //};
        //return await GetAsync<PaginationDto<StaffDto>>(nameof(GetStaffsAsync), paramters);
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

    public async Task DeleteStaffAsync(Guid id)
    {
        await DeleteAsync(nameof(DeleteStaffAsync), new RemoveStaffDto(id));
    }
}

