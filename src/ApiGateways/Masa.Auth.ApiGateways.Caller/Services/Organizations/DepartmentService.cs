using Masa.Auth.Contracts.Admin.Organizations;

namespace Masa.Auth.ApiGateways.Caller.Services.Organizations;

public class DepartmentService : ServiceBase
{
    string _baseUrl = "api/department";

    internal DepartmentService(ICallerProvider callerProvider) : base(callerProvider)
    {
    }

    public async Task<DepartmentDetailDto> GetAsync(Guid id)
    {
        return await GetAsync<DepartmentDetailDto>($"{_baseUrl}/Get?id={id}");
    }

    public async Task<List<DepartmentDto>> GetListAsync()
    {
        return await GetAsync<List<DepartmentDto>>($"{_baseUrl}/List");
    }

    public async Task UpsertAsync(UpsertDepartmentDto upsertDepartmentDto)
    {
        await PostAsync($"{_baseUrl}/Add", upsertDepartmentDto);
    }

    public async Task RemoveAsync(Guid departmentId)
    {
        await DeleteAsync($"{_baseUrl}/Remove?id={departmentId}");
    }

    public async Task<DepartmentChildrenCountDto> GetCountAsync()
    {
        return await GetAsync<DepartmentChildrenCountDto>($"{_baseUrl}/Count");
    }
}
