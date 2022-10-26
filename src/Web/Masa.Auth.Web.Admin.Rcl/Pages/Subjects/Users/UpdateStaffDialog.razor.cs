// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Newtonsoft.Json;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UpdateStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid StaffId { get; set; }

    private StaffDetailDto StaffDetail { get; set; } = new();

    private UpdateStaffDto Staff { get; set; } = new();

    private StaffService StaffService => AuthCaller.StaffService;

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
            await GetStaffDetailAsync();
        }
    }

    public async Task GetStaffDetailAsync()
    {
        StaffDetail = await StaffService.GetDetailAsync(StaffId);
        Staff = StaffDetail;
    }

    public async Task UpdateStaffAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await StaffService.UpdateAsync(Staff);
            OpenSuccessMessage(T("Edit staff data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }

    public async Task OpenRemoveStaffDialog()
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete staff data"));
        if (confirm) await RemoveStaffAsync();
    }

    public async Task RemoveStaffAsync()
    {
        Loading = true;
        await StaffService.RemoveAsync(StaffId);
        OpenSuccessMessage(T("Delete staff data success"));
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
        Loading = false;
    }
}

