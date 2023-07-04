// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations;

public class CommandHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly IPositionRepository _positionRepository;
    readonly IUnitOfWork _unitOfWork;

    public CommandHandler(
        IDepartmentRepository departmentRepository,
        IPositionRepository positionRepository,
        IUnitOfWork unitOfWork)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
        _unitOfWork = unitOfWork;
    }

    [EventHandler(1)]
    public async Task UpsertDepartmentAsync(UpsertDepartmentCommand command)
    {
        var dto = command.UpsertDepartmentDto;
        Expression<Func<Department, bool>> predicate = d => d.Name.Equals(dto.Name) && d.ParentId == dto.ParentId;
        predicate = predicate.And(dto.IsUpdate, d => d.Id != dto.Id);
        if (_departmentRepository.Any(predicate))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.DAPARTMENT_NAME_EXIST, dto.Name);
        }
        var parent = await _departmentRepository.FindAsync(dto.ParentId);
        if (dto.IsUpdate)
        {
            var department = await _departmentRepository.FindAsync(dto.Id);
            if (department is null)
            {
                throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DAPARTMENT_NOT_FOUND);
            }
            department.SetStaffs(dto.StaffIds.ToArray());
            department.Move(parent);
            department.Update(dto.Name, dto.Description, dto.Enabled);
            await _departmentRepository.UpdateAsync(department);
            command.Result = department!.Id;
            return;
        }
        var addDepartment = new Department(dto.Name, dto.Description, parent, dto.Enabled);
        addDepartment.SetStaffs(dto.StaffIds.ToArray());
        await _departmentRepository.AddAsync(addDepartment);
        await _unitOfWork.SaveChangesAsync();
        command.Result = addDepartment!.Id;
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

    [EventHandler(1)]
    public async Task CopyDepartmentAsync(CopyDepartmentCommand copyDepartmentCommand)
    {
        var dto = copyDepartmentCommand.CopyDepartmentDto;
        if (_departmentRepository.Any(d => d.Name.Equals(dto.Name)))
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.DAPARTMENT_NAME_EXIST, dto.Name);
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

    [EventHandler(1)]
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

    [EventHandler(1)]
    public async Task AddPositionAsync(AddPositionCommand command)
    {
        var position = await _positionRepository.FindAsync(p => p.Name == command.Position.Name);
        if (position is null)
        {
            position = new(command.Position.Name);
            await _positionRepository.AddAsync(position);
        }
        else throw new UserFriendlyException(UserFriendlyExceptionCodes.POSITION_NAMME_EXIST, command.Position.Name);
        command.Result = position.Id;
    }

    [EventHandler(1)]
    public async Task UpdatePositionAsync(UpdatePositionCommand command)
    {
        var positionDto = command.Position;
        var position = await _positionRepository.FindAsync(p => p.Id == positionDto.Id);
        if (position is null) throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.POSITION_NOT_EXIST);
        var existPosition = await _positionRepository.FindAsync(p => p.Id != positionDto.Id && p.Name == positionDto.Name);
        if (existPosition is not null)
        {
            throw new UserFriendlyException(UserFriendlyExceptionCodes.POSITION_NAMME_EXIST, command.Position.Name);
        }
        position.Update(positionDto.Name);
        await _positionRepository.UpdateAsync(position);
    }

    [EventHandler(1)]
    public async Task UpsertPositionAsync(UpsertPositionCommand command)
    {
        var position = await _positionRepository.FindAsync(p => p.Name == command.Name);
        if (position is null)
        {
            position = new(command.Name);
            await _positionRepository.AddAsync(position);
        }
        command.Result = position.Id;
    }

    [EventHandler(1)]
    public async Task RemovePositionAsync(RemovePositionCommand command)
    {
        var position = await _positionRepository.FindAsync(position => position.Id == command.Position.Id);
        if (position == null)
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.POSITION_NOT_EXIST);

        await _positionRepository.RemoveAsync(position);
    }
}

