﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Infrastructure.Constants;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class Organization
{
    List<Guid> _active = new List<Guid>();
    Guid _currentStaffId = Guid.Empty;
    List<DepartmentDto> _departments = new();
    bool _showAdd, _showCopy, _addStaff, _updateStaff;
    DepartmentChildrenCountDto _departmentChildrenCountDto = new();
    readonly List<DataTableHeader<StaffDto>> _headers = new()
    {
        new() { Text = "员工", Value = nameof(UserDto.Name), CellClass = "body-medium emphasis2--text" },
        new() { Text = "手机号", Value = nameof(UserDto.PhoneNumber), CellClass = "subtitle" },
        new() { Text = "邮箱", Value = nameof(UserDto.Email), CellClass = "subtitle" },
        new() { Text = "工号", Value = nameof(StaffDto.JobNumber), CellClass = "subtitle" },
        new() { Text = "操作", Value = "Action", Sortable = false, Align="center", Width="80px" }
    };
    PaginationDto<StaffDto> _paginationStaffs = new();
    UpsertDepartmentDto _upsertDepartmentDto = new();
    CopyDepartmentDto _copyDepartmentDto = new();
    GetStaffsDto _getStaffsDto = new GetStaffsDto(1, 10, "", Guid.Empty);
    DepartmentService DepartmentService => AuthCaller.DepartmentService;
    StaffService StaffService => AuthCaller.StaffService;

    [Parameter]
    public Guid DepartmentId { get; set; } = Guid.Empty;

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadDepartmentsAsync();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadDepartmentsAsync()
    {
        _departments = await DepartmentService.GetListAsync();
        _departmentChildrenCountDto = await DepartmentService.GetCountAsync();
    }

    private async Task LoadStaffsAsync(Guid departmentId)
    {
        _getStaffsDto.DepartmentId = departmentId;
        var data = await StaffService.GetListAsync(_getStaffsDto);
        _paginationStaffs = data;
    }

    private async Task LoadStaffsAsync()
    {
        var data = await StaffService.GetListAsync(_getStaffsDto);
        _paginationStaffs = data;
    }

    private void Add(Guid parentId)
    {
        _upsertDepartmentDto = new UpsertDepartmentDto();
        _upsertDepartmentDto.ParentId = parentId;
        _showAdd = true;
    }

    private async Task EnterSearch(KeyboardEventArgs eventArgs)
    {
        if (eventArgs.Key == Keyboards.Enter)
        {
            await LoadStaffsAsync();
        }
    }

    private async Task DeleteAsync(Guid departmentId)
    {
        await DepartmentService.RemoveAsync(departmentId);
        await LoadDepartmentsAsync();
        _showAdd = false;
    }

    private async Task Update(Guid departmentId)
    {
        var department = await DepartmentService.GetAsync(departmentId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _upsertDepartmentDto = new UpsertDepartmentDto();
        _upsertDepartmentDto.Id = department.Id;
        _upsertDepartmentDto.Name = department.Name;
        _upsertDepartmentDto.Description = department.Description;
        _upsertDepartmentDto.Enabled = department.Enabled;
        _upsertDepartmentDto.ParentId = department.ParentId;
        _showAdd = true;
    }

    private async Task SubmitAsync(UpsertDepartmentDto dto)
    {
        await DepartmentService.UpsertAsync(dto);
        await LoadDepartmentsAsync();
        _showAdd = false;
    }

    private async Task SubmitAsync(CopyDepartmentDto dto)
    {
        await DepartmentService.UpsertAsync(dto);
        await LoadDepartmentsAsync();
        _showCopy = false;
    }

    private async Task Copy(Guid sourceId)
    {
        var department = await DepartmentService.GetAsync(sourceId);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _copyDepartmentDto = new CopyDepartmentDto();
        _copyDepartmentDto.Name = department.Name;
        _copyDepartmentDto.Description = department.Description;
        _copyDepartmentDto.Enabled = department.Enabled;
        _copyDepartmentDto.ParentId = department.ParentId;
        _copyDepartmentDto.Staffs = department.StaffList;
        _showCopy = true;
    }

    private async Task ActiveUpdated(List<DepartmentDto> activedItems)
    {
        if (!activedItems.Any())
        {
            return;
        }
        await LoadStaffsAsync(activedItems.First().Id);
    }

    private void UpdateStaff(Guid staffId)
    {
        _currentStaffId = staffId;
        _updateStaff = true;
    }
}

