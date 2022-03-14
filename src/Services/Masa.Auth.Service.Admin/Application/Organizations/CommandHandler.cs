namespace Masa.Auth.Service.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;

    public CommandHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [EventHandler]
    public async Task CreateDepartmentAsync(AddDepartmentCommand createDepartmentCommand)
    {
        var department = DepartmentFactory.Create(createDepartmentCommand.Name, createDepartmentCommand.Description,
            createDepartmentCommand.ParentId, createDepartmentCommand.Enabled, createDepartmentCommand.StaffIds.ToArray());

        await _departmentRepository.AddAsync(department);
    }

    [EventHandler]
    public async Task CopyDepartmentAsync(CopyDepartmentCommand copyDepartmentCommand)
    {
        var department = DepartmentFactory.Create(copyDepartmentCommand.Name, copyDepartmentCommand.Description,
        copyDepartmentCommand.ParentId, copyDepartmentCommand.Enabled, copyDepartmentCommand.StaffIds.ToArray());

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

