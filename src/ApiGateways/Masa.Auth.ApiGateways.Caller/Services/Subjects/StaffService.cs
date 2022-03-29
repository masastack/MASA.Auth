namespace Masa.Auth.ApiGateways.Caller.Services.Subjects;

public class StaffService : ServiceBase
{
    string baseUrl = "api/staff";

    List<StaffDto> Staffs => new List<StaffDto>()
    {

    };

    internal StaffService(ICallerProvider callerProvider) : base(callerProvider)
    {

    }

    public async Task<PaginationDto<StaffDto>> GetStaffPaginationAsync(GetStaffsDto getStaffsDto)
    {
        var paramters = new Dictionary<string, string>()
        {
            { nameof(GetStaffsDto.Page),getStaffsDto.Page.ToString() },
            { nameof(GetStaffsDto.PageSize),getStaffsDto.PageSize.ToString() },
            { nameof(GetStaffsDto.Email),getStaffsDto.Email.ToString() },
            { nameof(GetStaffsDto.PhoneNumber),getStaffsDto.PhoneNumber.ToString() },
            { nameof(GetStaffsDto.DepartmentId),getStaffsDto.DepartmentId.ToString() },
            { nameof(GetStaffsDto.Name),getStaffsDto.Name.ToString() }
        };
        return await GetAsync<PaginationDto<StaffDto>>($"{baseUrl}/Pagination", paramters);
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

