using MASA.Auth.Service.Application.Organizations.Commands;

namespace MASA.Auth.Service.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;

    public CommandHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [EventHandler]
    public async Task CreateDepartmentAsync(CreateDepartmentCommand createDepartmentCommand)
    {
        var department = new Department(createDepartmentCommand.Name, createDepartmentCommand.Description);
        department.AddStaffs(createDepartmentCommand.StaffIds.ToArray());
        department.Move(createDepartmentCommand.ParentId);
        await _departmentRepository.AddAsync(department);
    }

    [EventHandler]
    public async Task DeleteDepartmentAsync(DeleteDepartmentCommand deleteDepartmentCommand)
    {
        var department = await _departmentRepository.FindAsync(deleteDepartmentCommand.DepartmentId);
        if (department is null)
        {
            throw new UserFriendlyException("The current department does not exist");
        }
        await _departmentRepository.RemoveAsync(department);
    }
}

