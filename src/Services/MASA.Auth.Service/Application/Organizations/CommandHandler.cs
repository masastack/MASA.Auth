namespace Masa.Auth.Service.Application.Organizations;

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
        department.SetEnabled(createDepartmentCommand.Enabled);
        await _departmentRepository.AddAsync(department);
    }

    [EventHandler]
    public async Task CopyDepartmentAsync(CopyDepartmentCommand copyDepartmentCommand)
    {
        var department = new Department(copyDepartmentCommand.Name, copyDepartmentCommand.Description);
        department.AddStaffs(copyDepartmentCommand.StaffIds.ToArray());
        department.Move(copyDepartmentCommand.ParentId);
        department.SetEnabled(copyDepartmentCommand.Enabled);
        await _departmentRepository.AddAsync(department);
        if (copyDepartmentCommand.IsMigrate)
        {
            var originalDepartment = await _departmentRepository.FindAsync(copyDepartmentCommand.OriginalId);
            if (originalDepartment is null)
            {
                throw new UserFriendlyException("The original department does not exist");
            }
            originalDepartment.RemoveStaffs(copyDepartmentCommand.StaffIds.ToArray());
            await _departmentRepository.UpdateAsync(originalDepartment);
        }
    }

    [EventHandler]
    public async Task DeleteDepartmentAsync(DeleteDepartmentCommand deleteDepartmentCommand)
    {
        var department = await _departmentRepository.FindAsync(deleteDepartmentCommand.DepartmentId);
        if (department is null)
        {
            throw new UserFriendlyException("The current department does not exist");
        }
        if (department.DepartmentStaffs.Any())
        {
            throw new UserFriendlyException("The current department has staff,delete failed");
        }

        await _departmentRepository.RemoveAsync(department);
    }
}

