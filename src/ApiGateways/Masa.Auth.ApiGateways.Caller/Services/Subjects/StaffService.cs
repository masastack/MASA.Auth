namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    List<StaffDto> Staffs => new List<StaffDto>()
    {

    };

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<StaffDto>> GetStaffsAsync(GetStaffsDto request)
    {
        var staffs = Staffs.Skip((request.Page - 1) * request.PageSize).Take(request.PageSize).ToList();
        return await Task.FromResult(new PaginationDto<StaffDto>(Staffs.Count, 1, staffs));
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
        Staffs.Remove(Staffs.First(s => s.Id == staffId));
        await Task.CompletedTask;
    }
}

