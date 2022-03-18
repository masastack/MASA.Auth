namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    List<StaffDto> StaffItems => new List<StaffDto>()
    {

    };

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<StaffDto>> GetStaffItemsAsync(GetStaffsDto request)
    {
        var staffs = StaffItems.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<StaffDto>(StaffItems.Count, 1, staffs));
    }

    public async Task<StaffDetailDto> GetStaffDetailAsync(Guid id)
    {
        return await Task.FromResult(StaffDetailDto.Default);
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
        StaffItems.Remove(StaffItems.First(s => s.StaffId == staffId));
        await Task.CompletedTask;
    }
}

