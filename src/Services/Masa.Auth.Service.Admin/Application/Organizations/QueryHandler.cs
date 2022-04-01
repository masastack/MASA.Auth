namespace Masa.Auth.Service.Admin.Application.Organizations;

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
        departmentDetailQuery.Result = new DepartmentDetailDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Enabled = department.Enabled,
            ParentId = department.ParentId,
            StaffList = department.DepartmentStaffs
                .Select(ds => ds.Staff)
                .Select(s => new StaffDto(s.Id, department.Name, "",s.JobNumber,s.Enabled, s.User.Name, s.User.DisplayName, s.User.Avatar, s.User.PhoneNumber, s.User.Email)).ToList()
        };
    }

    [EventHandler]
    public async Task GetDepartmentTreeAsync(DepartmentTreeQuery departmentTreeQuery)
    {
        departmentTreeQuery.Result = await GetDepartmentsAsync(departmentTreeQuery.ParentId);
    }

    private async Task<List<DepartmentDto>> GetDepartmentsAsync(Guid parentId)
    {
        var result = new List<DepartmentDto>();
        //todo change memory
        var departments = await _departmentRepository.GetListAsync(d => d.ParentId == parentId);
        foreach (var department in departments)
        {
            var item = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Children = await GetDepartmentsAsync(department.Id),
                IsRoot = department.Level == 1
            };
            result.Add(item);
        }
        return result;
    }

    [EventHandler]
    public async Task DepartmentCountAsync(DepartmentCountQuery departmentCountQuery)
    {
        departmentCountQuery.Result = new DepartmentChildrenCountDto
        {
            SecondLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 2)),
            ThirdLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 3)),
            FourthLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 4)),
        };
    }
}

