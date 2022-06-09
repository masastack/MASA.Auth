// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Application.Organizations;

public class QueryHandler
{
    readonly IDepartmentRepository _departmentRepository;
    readonly IPositionRepository _positionRepository;

    public QueryHandler(IDepartmentRepository departmentRepository, IPositionRepository positionRepository)
    {
        _departmentRepository = departmentRepository;
        _positionRepository = positionRepository;
    }

    [EventHandler]
    public async Task GetDepartmentDetailAsync(DepartmentDetailQuery departmentDetailQuery)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentDetailQuery.DepartmentId);
        departmentDetailQuery.Result = new DepartmentDetailDto
        {
            Id = department.Id,
            Name = department.Name,
            Description = department.Description,
            Enabled = department.Enabled,
            ParentId = department.ParentId,
            StaffList = department.DepartmentStaffs
                .Select(ds => ds.Staff)
                .Select(staff => (StaffDto)staff).ToList()
        };
    }

    [EventHandler]
    public async Task GetDepartmentTreeAsync(DepartmentTreeQuery departmentTreeQuery)
    {
        departmentTreeQuery.Result = await GetDepartmentsAsync(departmentTreeQuery.ParentId);
    }

    private async Task<List<DepartmentDto>> GetDepartmentsAsync(Guid parentId)
    {
        var result = new List<DepartmentDto>();
        //todo change memory
        var departments = await _departmentRepository.GetListAsync(d => d.ParentId == parentId);
        foreach (var department in departments)
        {
            var item = new DepartmentDto
            {
                Id = department.Id,
                Name = department.Name,
                Children = await GetDepartmentsAsync(department.Id),
                IsRoot = department.Level == 1
            };
            result.Add(item);
        }
        return result;
    }

    [EventHandler]
    public async Task DepartmentCountAsync(DepartmentCountQuery departmentCountQuery)
    {
        departmentCountQuery.Result = new DepartmentChildrenCountDto
        {
            SecondLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 2)),
            ThirdLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 3)),
            FourthLevel = (int)(await _departmentRepository.GetCountAsync(d => d.Level == 4)),
        };
    }

    [EventHandler]
    public async Task GetPositionsAsync(PositionsQuery query)
    {
        Expression<Func<Position, bool>> condition = position => true;
        if (string.IsNullOrEmpty(query.Search) is false)
            condition = condition.And(position => position.Name.Contains(query.Search));

        var positions = await _positionRepository.GetPaginatedListAsync(condition, new PaginatedOptions
        {
            Page = query.Page,
            PageSize = query.PageSize,
            Sorting = new Dictionary<string, bool>
            {
                [nameof(Position.ModificationTime)] = true,
                [nameof(Position.CreationTime)] = true,
            }
        });

        query.Result = new(positions.Total, positions.Result.Select(position => new PositionDto(position.Id, position.Name)).ToList());
    }

    [EventHandler]
    public async Task GetPositionDetailAsync(PositionDetailQuery query)
    {
        var position = await _positionRepository.FindAsync(query.PositionId);
        if (position is null) throw new UserFriendlyException("This position data does not exist");

        query.Result = new PositionDetailDto(position.Id, position.Name);
    }

    [EventHandler]
    public async Task GetDepartmentSelectAsync(DepartmentSelectQuery query)
    {
        var departments = await _departmentRepository.GetListAsync(department => department.Name.Contains(query.Name));
        query.Result = departments.Select(department => new DepartmentSelectDto(department.Id, department.Name)).ToList();
    }

    [EventHandler]
    public async Task GetPositionSelectAsync(PositionSelectQuery query)
    {
        var psoitions = await _positionRepository.GetListAsync(p => p.Name.Contains(query.Name));
        query.Result = psoitions.Select(p => new PositionSelectDto(p.Id, p.Name)).ToList();
    }
}

