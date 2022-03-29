namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;

    public CommandHandler(IDepartmentRepository departmentRepository)
    {
        _departmentRepository = departmentRepository;
    }

    [EventHandler]
    public async Task AddDepartmentAsync(AddDepartmentCommand createDepartmentCommand)
    {
        var dto = createDepartmentCommand.AddOrUpdateDepartmentDto;
        var parent = await _departmentRepository.FindAsync(dto.ParentId);
        // Add
        if (dto.Id == Guid.Empty)
        {
            var addDepartment = new Department(dto.Name, dto.Description, parent, dto.Enabled);
            addDepartment.AddStaffs(dto.StaffIds.ToArray());
            await _departmentRepository.AddAsync(addDepartment);
            return;
        }
        //update
        var department = await _departmentRepository.FindAsync(dto.Id);
        if (department is null)
        {
            throw new UserFriendlyException($"current department id {dto.Id} not found");
        }
        department.ResetStaffs(dto.StaffIds.ToArray());
        department.Update(dto.Name, dto.Description, dto.Enabled);
        if (parent != null)
        {
            department.Move(parent);
        }
        await _departmentRepository.UpdateAsync(department);
    }

    [EventHandler]
    public async Task RemoveDepartmentAsync(RemoveDepartmentCommand deleteDepartmentCommand)
    {
        var department = await _departmentRepository.GetByIdAsync(deleteDepartmentCommand.DepartmentId);
        await RemoveCheckAsync(department);
    }

    private async Task RemoveCheckAsync(Department department)
    {
        department.DeleteCheck();
        var childDepartments = await _departmentRepository.QueryListAsync(d => d.ParentId == department.Id);
        foreach (var childDepartment in childDepartments)
        {
            await RemoveCheckAsync(childDepartment);
        }
        await _departmentRepository.RemoveAsync(department);
    }
}

