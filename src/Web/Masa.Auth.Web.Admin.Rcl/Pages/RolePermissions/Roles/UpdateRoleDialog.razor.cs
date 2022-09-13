// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class UpdateRoleDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid RoleId { get; set; }

    private UpdateRoleDto Role { get; set; } = new();

    private RoleDetailDto RoleDetail { get; set; } = new();

    private RoleService RoleService => AuthCaller.RoleService;

    private MForm? Form { get; set; }

    public bool Preview { get; set; }

    private UpdateRoleTabs Tab { get; set; } = UpdateRoleTabs.BasicInformation;

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
            Tab = UpdateRoleTabs.BasicInformation;
            await GetRoleDetailAsync();
        }
    }

    public async Task GetRoleDetailAsync()
    {
        RoleDetail = await RoleService.GetDetailAsync(RoleId);
        Role = RoleDetail;
    }

    public async Task UpdateRoleAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await RoleService.UpdateAsync(Role);
            OpenSuccessMessage(T("Edit role data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    private void LimitChanged(int limit)
    {
        if (limit != 0)
        {
            var availableQuantity = limit + RoleDetail.AvailableQuantity - RoleDetail.Limit;
            if (availableQuantity < 0)
            {
                OpenWarningMessage(T("The number of bindings cannot be less than ") + (RoleDetail.Limit - RoleDetail.AvailableQuantity));
            }
        }
    }
}

