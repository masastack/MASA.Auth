namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IdepartmentRepository _departmentRepository;

    public CommandHandler(IdepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [EventHandler]
    public async Task CreateDepartmentAsync(AddDepartmentCommand createDepartmentCommand)
    {
        var parent = await _departmentRepository.FindAsync(createDepartmentCommand.ParentId);
        var department = new Department(createDepartmentCommand.Name, createDepartmentCommand.Description,
            parent, createDepartmentCommand.Enabled, createDepartmentCommand.StaffIds.ToArray());

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

