namespace Masa.Auth.Service.Admin.Domain.Organizations.Factories;

public class DepartmentFactory : IAggregateFactory
{
    readonly IDepartmentRepository _departmentRepository;

    public DepartmentFactory(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    public async Task<Department> CreateAsync(string name, string description, Guid parentId, bool enabled, Guid[] staffIds)
    {
        var department = new Department(name, description);
        department.SetEnabled(enabled);
        department.AddStaffs(staffIds);
        var parentDepartment = await _departmentRepository.FindAsync(parentId);
        if (parentDepartment != null)
        {
            department.Move(parentDepartment);
        }
        return department;
    }
}
