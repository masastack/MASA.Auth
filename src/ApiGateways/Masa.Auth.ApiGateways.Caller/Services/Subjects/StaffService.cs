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
    }

    public async Task<StaffDetailDto> GetStaffDetailAsync(Guid id)
    {
        return await Task.FromResult(new StaffDetailDto());
    }

    public async Task AddStaffAsync(AddStaffDto request)
    {
        await Task.CompletedTask;
    }

    public async Task UpdateStaffAsync(UpdateStaffDto request)
    {
        await Task.CompletedTask;
    }

    public async Task DeleteStaffAsync(Guid staffId)
    {
        Staffs.Remove(Staffs.First(s => s.Id == staffId));
        await Task.CompletedTask;
    }
}

