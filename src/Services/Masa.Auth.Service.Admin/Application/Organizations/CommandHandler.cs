namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly DepartmentFactory _departmentFactory;

    public CommandHandler(IDepartmentRepository departmentRepository, DepartmentFactory departmentFactory)
    {
        _departmentRepository = departmentRepository;
        _departmentFactory = departmentFactory;
    }

    [EventHandler]
    public async Task CreateDepartmentAsync(AddDepartmentCommand createDepartmentCommand)
    {
        var department = await _departmentFactory.CreateAsync(createDepartmentCommand.Name, createDepartmentCommand.Description,
            createDepartmentCommand.ParentId, createDepartmentCommand.Enabled, createDepartmentCommand.StaffIds.ToArray());

        await _departmentRepository.AddAsync(department);
    }

    [EventHandler]
    public async Task DeleteDepartmentAsync(RemoveDepartmentCommand deleteDepartmentCommand)
    {
        var department = await _departmentRepository.GetByIdAsync(deleteDepartmentCommand.DepartmentId);
        await DeleteCheckAsync(department);
    }

    private async Task DeleteCheckAsync(Department department)
    {
        department.DeleteCheck();
        var childDepartments = await _departmentRepository.QueryListAsync(d => d.ParentId == department.Id);
        foreach (var childDepartment in childDepartments)
        {
            await DeleteCheckAsync(childDepartment);
        }
        await _departmentRepository.RemoveAsync(department);
    }
}

