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

    [EventHandler]
    public async Task DepartmentStaffAsync(DepartmentStaffQuery departmentStaffQuery)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentStaffQuery.DepartmentId);
        departmentStaffQuery.Result = department.DepartmentStaffs
            .Select(ds => ds.Staff)
            .Select(s => new StaffItem
            {
                Id = s.Id,
                Name = s.User.Name,
                JobNumber = s.JobNumber,
                Email = s.User.Email,
                PhoneNumber = s.User.PhoneNumber
            }).ToList();
    }

    [EventHandler]
    public async Task DepartmentCountAsync(DepartmentCountQuery departmentCountQuery)
    {
        departmentCountQuery.Result = new DepartmentCount
        {
            SecondLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 2)),
            ThreeLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 3)),
            FourLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 4)),
        };
    }
}

