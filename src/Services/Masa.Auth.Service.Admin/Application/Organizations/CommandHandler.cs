using Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;
using Masa.Auth.Service.Admin.Domain.Organizations.Factories;
using Masa.Auth.Service.Admin.Domain.Organizations.Repositories;

namespace Masa.Auth.Service.Admin.Application.Organizations;

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

