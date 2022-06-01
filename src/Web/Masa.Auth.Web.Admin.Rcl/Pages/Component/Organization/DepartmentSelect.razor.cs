// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class DepartmentSelect
{
    private string? Text { get; set; }

    private bool Visible { get; set; }

    [Parameter]
    public Guid Value { get; set; }

    [Parameter]
    public EventCallback<Guid> ValueChanged { get; set; }

    public List<DepartmentDto> Departments { get; set; } = new();

    private DepartmentService DepartmentService => AuthCaller.DepartmentService;

    protected override async Task OnInitializedAsync()
    {
        Departments = await DepartmentService.GetListAsync();
    }

    private async Task OnValueChanged(DepartmentDto department)
    {
        Text = department.Name;
        Value = department.Id;
        await ValueChanged.InvokeAsync(Value);
    }
}

