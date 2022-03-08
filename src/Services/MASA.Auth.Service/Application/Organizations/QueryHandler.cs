using Masa.Auth.Service.Domain.Organizations.Repositories;

namespace Masa.Auth.Service.Application.Organizations;

public class QueryHandler
{
    readonly IDepartmentRepository _departmentRepository;

    public QueryHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [EventHandler]
    public async Task GetDepartmentDetailAsync(DepartmentDetailQuery departmentDetailQuery)
    {

    }

    [EventHandler]
    public async Task GetDepartmentTreeAsync(DepartmentTreeQuery departmentTreeQuery)
    {

    }
}

