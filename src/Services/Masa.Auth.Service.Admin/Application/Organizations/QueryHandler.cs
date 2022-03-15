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
        var department = await _departmentRepository.GetByIdAsync(departmentDetailQuery.DepartmentId);
        departmentDetailQuery.Result = new DepartmentDetail
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Enabled = department.Enabled
        };
    }

    [EventHandler]
    public async Task GetDepartmentTreeAsync(DepartmentTreeQuery departmentTreeQuery)
    {
        departmentTreeQuery.Result = await GetDepartmentsAsync(departmentTreeQuery.ParentId);
    }

    private async Task<List<DepartmentItem>> GetDepartmentsAsync(Guid parentId)
    {
        var result = new List<DepartmentItem>();
        //todo change memory
        var departments = await _departmentRepository.GetListAsync(d => d.ParentId == parentId);
        foreach (var department in departments)
        {
            var item = new DepartmentItem
            {
                Id = department.Id,
                Name = department.Name,
                Children = await GetDepartmentsAsync(department.Id)
            };
            result.Add(item);
        }
        return result;
    }
}

