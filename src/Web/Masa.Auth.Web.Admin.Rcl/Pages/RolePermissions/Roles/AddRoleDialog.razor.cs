// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class AddRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddRoleDto Role { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    private RoleSelectForRole? RoleSelect { get; set; }

    public bool Preview { get; set; }

    private int Step { get; set; } = 1;

    protected override string? PageName { get; set; } = "RoleBlock";

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            Role = new();
            if (RoleSelect is not null) await RoleSelect.ReloadAsync();
            Step = 1;
        }
    }

    public async Task AddRoleAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await RoleService.AddAsync(Role);
            OpenSuccessMessage(T("Add role data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    void NextStep(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Step = 2;
        }
    }

    void NameChanged(string name)
    {
        if (string.IsNullOrEmpty(Role.Code) || Role.Code == Role.Name.ToSpellCode())
        {
            Role.Code = name.ToSpellCode();
        }
        Role.Name = name;
    }
}

