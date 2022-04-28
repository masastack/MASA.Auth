namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly IPositionRepository _positionRepository;

    public CommandHandler(IDepartmentRepository departmentRepository, IPositionRepository positionRepository)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
    }

    [EventHandler]
    public async Task UpsertDepartmentAsync(AddDepartmentCommand addDepartmentCommand)
    {
        var dto = addDepartmentCommand.UpsertDepartmentDto;
        var parent = await _departmentRepository.FindAsync(dto.ParentId);
        if (dto.IsUpdate)
        {
            var department = await _departmentRepository.FindAsync(dto.Id);
            if (department is null)
            {
                throw new UserFriendlyException($"current department id {dto.Id} not found");
            }
            department.SetStaffs(dto.StaffIds.ToArray());
            department.Update(dto.Name, dto.Description, dto.Enabled);
            if (parent != null)
            {
                department.Move(parent);
            }
            await _departmentRepository.UpdateAsync(department);
            return;
        }
        var addDepartment = new Department(dto.Name, dto.Description, parent, dto.Enabled);
        addDepartment.SetStaffs(dto.StaffIds.ToArray());
        await _departmentRepository.AddAsync(addDepartment);
    }

    [EventHandler]
    public async Task RemoveDepartmentAsync(RemoveDepartmentCommand removeDepartmentCommand)
    {
        var department = await _departmentRepository.GetByIdAsync(removeDepartmentCommand.DepartmentId);
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

    [EventHandler]
    public async Task UpsertPosition(UpsertPositionCommand command)
    {
        var positionDto = command.Position;
        var exist = await _positionRepository.GetCountAsync(p => p.Name == positionDto.Name) > 0;
        if (exist) throw new UserFriendlyException($"Position with name {positionDto.Name} already exists");

        var position = await _positionRepository.FindAsync(p => p.Id == positionDto.Id);
        if (position is null)
        {
            position = new Position(command.Position.Name);
            await _positionRepository.AddAsync(position);
        }
        else
        {
            position.Update(command.Position.Name);
            await _positionRepository.UpdateAsync(position);
        }

        command.Position.Id = position.Id;
    }
}

