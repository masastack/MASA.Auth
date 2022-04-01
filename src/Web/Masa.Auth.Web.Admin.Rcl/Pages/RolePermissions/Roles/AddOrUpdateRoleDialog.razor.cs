﻿namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class AddOrUpdateRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid RoleId { get; set; }

    private bool IsAdd => RoleId == Guid.Empty;

    private RoleDetailDto Role { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

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
            if (IsAdd) Role = new();
            else await GetRoleDetailAsync();
        }
    }

    public async Task GetRoleDetailAsync()
    {
        Role = await RoleService.GetRoleDetailAsync(RoleId);
    }

    public async Task AddOrEditRoleAsync()
    {
        Loading = true;
        if (IsAdd)
        {
            await RoleService.AddRoleAsync(Role);
            OpenSuccessMessage(T("Add role data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        else
        {
            await RoleService.UpdateRoleAsync(Role);
            OpenSuccessMessage(T("Edit role data success"));
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
        }
        Loading = false;
    }

    protected override bool ShouldRender()
    {
        return Visible;
    }
}

