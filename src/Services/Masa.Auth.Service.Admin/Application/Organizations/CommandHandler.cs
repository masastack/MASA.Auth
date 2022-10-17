// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly IPositionRepository _positionRepository;
    readonly ILogger<CommandHandler> _logger;

    public CommandHandler(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        ILogger<CommandHandler> logger)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _logger = logger;
    }

    [EventHandler(1)]
    public async Task UpsertDepartmentAsync(UpsertDepartmentCommand command)
    {
        var dto = command.UpsertDepartmentDto;
        Expression<Func<Department, bool>> predicate = d => d.Name.Equals(dto.Name) && d.ParentId == dto.ParentId;
        if (dto.IsUpdate)
        {
            predicate = predicate.And(d => d.Id != dto.Id);
        }
        if (_departmentRepository.Any(predicate))
        {
            throw new UserFriendlyException($"The department name {dto.Name} already exists");
        }
        var parent = await _departmentRepository.FindAsync(dto.ParentId);
        if (dto.IsUpdate)
        {
            var department = await _departmentRepository.FindAsync(dto.Id);
            if (department is null)
            {
                throw new UserFriendlyException($"current department id {dto.Id} not found");
            }
            department.SetStaffs(dto.StaffIds.ToArray());
            department.Move(parent);
            department.Update(dto.Name, dto.Description, dto.Enabled);
            await _departmentRepository.UpdateAsync(department);
            return;
        }
        var addDepartment = new Department(dto.Name, dto.Description, parent, dto.Enabled);
        addDepartment.SetStaffs(dto.StaffIds.ToArray());
        await _departmentRepository.AddAsync(addDepartment);
    }

    [EventHandler(2)]
    public async Task UpdateChildLevelAsync(UpsertDepartmentCommand command)
    {
        var dto = command.UpsertDepartmentDto;
        if (dto.IsUpdate)
        {
            await UpdateLevel(dto.Id);
        }

        async Task UpdateLevel(Guid parentId)
        {
            //todo optimization
            var department = await _departmentRepository.FindAsync(parentId);
            var children = await _departmentRepository.GetListAsync(d => d.ParentId == parentId);
            foreach (var child in children)
            {
                child.Move(department);
                await UpdateLevel(child.Id);
            }
            await _departmentRepository.UpdateRangeAsync(children);
        }
    }

    [EventHandler]
    public async Task CopyDepartmentAsync(CopyDepartmentCommand copyDepartmentCommand)
    {
        var dto = copyDepartmentCommand.CopyDepartmentDto;
        if (_departmentRepository.Any(d => d.Name.Equals(dto.Name)))
        {
            throw new UserFriendlyException($"the department name {dto.Name} already exists");
        }
        var sourceDepartment = await _departmentRepository.GetByIdAsync(dto.SourceId);
        if (sourceDepartment != null)
        {
            sourceDepartment.RemoveStaffs(dto.StaffIds.ToArray());
            await _departmentRepository.UpdateAsync(sourceDepartment);
        }
        var parent = await _departmentRepository.FindAsync(dto.ParentId);
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
    public async Task AddPositionAsync(AddPositionCommand command)
    {
        var position = await _positionRepository.FindAsync(p => p.Name == command.Position.Name);
        if (position is null)
        {
            position = new(command.Position.Name);
            await _positionRepository.AddAsync(position);
        }
        else throw new UserFriendlyException($"Position with name {command.Position.Name} already exists");
        command.Result = position.Id;
    }

    [EventHandler]
    public async Task UpdatePositionAsync(UpdatePositionCommand command)
    {
        var positionDto = command.Position;
        var position = await _positionRepository.FindAsync(p => p.Id == positionDto.Id);
        if (position is null) throw new UserFriendlyException($"Current position not found");
        var existPosition = await _positionRepository.FindAsync(p => p.Id != positionDto.Id && p.Name == positionDto.Name);
        if (existPosition is not null)
        {
            throw new UserFriendlyException($"Position with name {command.Position.Name} already exists");
        }
        position.Update(positionDto.Name);
        await _positionRepository.UpdateAsync(position);
    }

    [EventHandler]
    public async Task UpsertPositionAsync(UpsertPositionCommand command)
    {
        var position = await _positionRepository.FindAsync(p => p.Name == command.Position.Name);
        if (position is null)
        {
            position = new(command.Position.Name);
            await _positionRepository.AddAsync(position);
        }
        command.Result = position.Id;
    }

    [EventHandler]
    public async Task RemovePositionAsync(RemovePositionCommand command)
    {
        var position = await _positionRepository.FindAsync(position => position.Id == command.Position.Id);
        if (position == null)
            throw new UserFriendlyException("The current position does not exist");

        await _positionRepository.RemoveAsync(position);
    }
}

