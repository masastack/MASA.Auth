// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class CopyOrgSheet
{
    [Parameter]
    public CopyDepartmentDto Dto { get; set; } = new();

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    [Parameter]
    public EventCallback<CopyDepartmentDto> OnSubmit { get; set; }

    StringNumber _step = 1;
    List<StaffDto> _removeStaffs = new();
    bool _visible;

    private void NextStep()
    {
        _step = 2;
        if (!Dto.MigrateStaff)
        {
            _removeStaffs.AddRange(Dto.Staffs);
            Dto.Staffs.Clear();
        }
        else
        {
            Dto.Staffs.AddRange(_removeStaffs);
            _removeStaffs.Clear();
        }
    }

    private async Task OnSubmitHandler()
    {
        if (OnSubmit.HasDelegate)
        {
            await OnSubmit.InvokeAsync(Dto);
        }
        _visible = false;
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

    public void Show(CopyDepartmentDto model)
    {
        Dto = model;
        Dto.Name = model.Name + "副本";
        _step = 1;
        _removeStaffs = new();
        _visible = true;
    }
}