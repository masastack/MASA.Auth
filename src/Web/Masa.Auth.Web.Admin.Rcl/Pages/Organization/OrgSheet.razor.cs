// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class OrgSheet
{
    [Parameter]
    public EventCallback<UpsertDepartmentDto> OnSubmit { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    MForm _form = default!;
    bool _visible;
    string _title = "", _saveTitle = "";
    UpsertDepartmentDto _dto = new();

    DepartmentService DepartmentService => AuthCaller.DepartmentService;

    private async Task OnSubmitHandler()
    {
        if (_form.Validate())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(_dto);
            }
            _visible = false;
        }
    }

    private async Task OnDeleteHandler()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(_dto.Id);
        }
        _visible = false;
    }

    public void Add(Guid parentId)
    {
        _dto = new();
        _dto.ParentId = parentId;
        _title = T("Add Department");
        _saveTitle = T("Submit");
        _visible = true;
    }

    public async Task Update(Guid id)
    {
        var department = await DepartmentService.GetAsync(id);
        if (department == null)
        {
            throw new UserFriendlyException("department id not found");
        }
        _dto = new UpsertDepartmentDto();
        _dto.Id = department.Id;
        _dto.Name = department.Name;
        _dto.Description = department.Description;
        _dto.Enabled = department.Enabled;
        _dto.ParentId = department.ParentId;

        _title = T("Edit Department");
        _saveTitle = T("Save");
        _visible = true;
    }
}