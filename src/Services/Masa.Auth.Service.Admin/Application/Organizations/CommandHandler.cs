// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

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
    public async Task CopyDepartmentAsync(CopyDepartmentCommand copyDepartmentCommand)
    {
        var dto = copyDepartmentCommand.CopyDepartmentDto;
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
        command.Result = position.Id;
    }

    [EventHandler]
    public async Task UpdatePositionAsync(UpdatePositionCommand command)
    {
        var positionDto = command.Position;
        var position = await _positionRepository.FindAsync(p => p.Id == positionDto.Id);
        if (position is null) throw new UserFriendlyException($"Current position not found");

        position.Update(positionDto.Name);
        await _positionRepository.UpdateAsync(position);
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

