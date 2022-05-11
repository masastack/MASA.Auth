// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Organization;

public partial class OrgSheet
{
    [Parameter]
    public bool Show { get; set; }

    [Parameter]
    public EventCallback<bool> ShowChanged { get; set; }

    [EditorRequired]
    [Parameter]
    public UpsertDepartmentDto Dto { get; set; } = new();

    [Parameter]
    public EventCallback<UpsertDepartmentDto> OnSubmit { get; set; }

    [Parameter]
    public EventCallback<Guid> OnDelete { get; set; }

    [Parameter]
    public List<DepartmentDto> Departments { get; set; } = new();

    MForm _form = default!;


    public async Task OnSubmitHandler()
    {
        if (await _form.ValidateAsync())
        {
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync(Dto);
            }
        }
    }

    public async Task OnDeleteHandler()
    {
        if (OnDelete.HasDelegate)
        {
            await OnDelete.InvokeAsync(Dto.Id);
        }
    }
}