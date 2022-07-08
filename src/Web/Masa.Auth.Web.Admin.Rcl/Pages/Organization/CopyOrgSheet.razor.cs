// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class CopyOrgSheet
{
    StringNumber _step = 1;
    List<StaffDto> _removeStaffs = new();

    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [EditorRequired]
    [Parameter]
    public CopyDepartmentDto Dto { get; set; } = new();

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback<CopyDepartmentDto> OnSubmit { get; set; }

    private void NextStep()
    {
        _step = 2;
        if (!Dto.MigrateStaff)
        {
            _removeStaffs.AddRange(Dto.Staffs);
            Dto.Staffs.Clear();
        }
    }

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(Dto);
        }
        await Toggle(false);
    }

    private void RemoveStaff(StaffDto staffDto)
    {
        Dto.Staffs.Remove(staffDto);
        _removeStaffs.Add(staffDto);
    }

    private void AddStaff(StaffDto staffDto)
    {
        _removeStaffs.Remove(staffDto);
        Dto.Staffs.Add(staffDto);
    }

    private async Task Toggle(bool value)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(value);
        }
        else
        {
            Visible = value;
        }
    }

    public async Task Show(CopyDepartmentDto model)
    {
        _step = 1;
        await Toggle(true);
    }
}