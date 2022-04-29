// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class CopyOrgSheet
{
    StringNumber _step = 1;
    List<StaffDto> _removeStaffs = new();

    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

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

    public async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(Dto);
        }
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
}