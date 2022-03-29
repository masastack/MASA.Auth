using Masa.Auth.Contracts.Admin.Organizations;

namespace Masa.Auth.ApiGateways.Caller.Services.Organizations;

public class DepartmentService : ServiceBase
{
    string baseUrl = "api/department";

    internal DepartmentService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<DepartmentDetailDto> GetAsync(Guid id)
    {
        return await GetAsync<DepartmentDetailDto>($"{baseUrl}/Get?id={id}");
    }

    public async Task<List<DepartmentDto>> GetListAsync()
    {
        return await GetAsync<List<DepartmentDto>>($"{baseUrl}/List");
    }

    public async Task AddOrUpdateAsync(AddOrUpdateDepartmentDto addOrUpdateDepartmentDto)
    {
        await PostAsync($"{baseUrl}/Add", addOrUpdateDepartmentDto);
    }

    public async Task RemoveAsync(Guid departmentId)
    {
        await DeleteAsync($"{baseUrl}/Remove?id={departmentId}");
    }

    public async Task<DepartmentChildrenCountDto> GetCountAsync()
    {
        return await GetAsync<DepartmentChildrenCountDto>($"{baseUrl}/Count");
    }
}
