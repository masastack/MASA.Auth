﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Organizations.Aggregates;

public class Department : FullAggregateRoot<Guid, Guid>
{
    public string Name { get; private set; }

    public Guid ParentId { get; private set; }

    public bool Enabled { get; private set; } = true;

    public int Level { get; private set; } = 1;

    public int Sort { get; private set; }

    public string Description { get; private set; } = string.Empty;

    private List<DepartmentStaff> _departmentStaffs = new();

    public IReadOnlyCollection<DepartmentStaff> DepartmentStaffs => _departmentStaffs;

    public Department(string name, string description) : this(name, description, null, true)
    {
    }

    public Department(string name, string description, Department? parent, bool enabled)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
        if (parent != null)
        {
            Move(parent);
        }
    }

    public void SetStaffs(params Guid[] staffIds)
    {
        _departmentStaffs = _departmentStaffs.MergeBy(
            staffIds.Select(staffId => new DepartmentStaff(staffId)),
            item => item.StaffId);
    }

    public void ResetStaffs(params Guid[] staffIds)
    {
        _departmentStaffs.Clear();
        foreach (var staffId in staffIds)
        {
            _departmentStaffs.Add(new DepartmentStaff(staffId));
        }
    }

    public void RemoveStaffs(params Guid[] staffIds)
    {
        _departmentStaffs.RemoveAll(ds => staffIds.Contains(ds.StaffId));
    }

    public void Move(Department? parent)
    {
        if (parent is null)
        {
            return;
        }
        ParentId = parent.Id;
        Level = parent.Level + 1;
    }

    public void Update(string name, string description, bool enabled)
    {
        Name = name;
        Description = description;
        Enabled = enabled;
    }

    public void DeleteCheck()
    {
        if (Level == 1)
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DEPARTMENT_ROOT_DELETE);
        }
        if (_departmentStaffs.Any())
        {
            throw new UserFriendlyException(errorCode: UserFriendlyExceptionCodes.DEPARTMENT_HAS_STAFF_DELETE);
        }
    }
}

