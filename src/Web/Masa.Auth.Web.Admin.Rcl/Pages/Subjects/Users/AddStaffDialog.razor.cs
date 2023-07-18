// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddStaffDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid? DepartmentId { get; set; }

    private int Step { get; set; } = 1;

    private AddStaffDto Staff { get; set; } = new();

    private StaffDefaultPasswordDto DefaultPasswordDto { get; set; } = new();

    private StaffService StaffService => AuthCaller.StaffService;

    private MForm _mForm = default!;

    protected override void OnInitialized()
    {
        PageName = "StaffBlock";
        base.OnInitialized();
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            Staff = new();
            if (DepartmentId.HasValue)
            {
                Staff.DepartmentId = DepartmentId.Value;
            }
            var defaultPasswordDto = await StaffService.GetDefaultPasswordAsync();
            Staff.Password = defaultPasswordDto.Enabled ? defaultPasswordDto.DefaultPassword : "";
            Step = 1;
        }
    }

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

    public async Task AddStaffAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            await StaffService.AddAsync(Staff);
            OpenSuccessMessage(T("Add staff data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
        }
    }

    public async Task SetDefaultPassword(EditContext context)
    {
        var field = context.Field(nameof(Staff.Password));
        context.NotifyFieldChanged(field);
        var result = context.GetValidationMessages(field);
        if (result.Any() is false)
        {
            DefaultPasswordDto.DefaultPassword = Staff.Password!;
            DefaultPasswordDto.Enabled = true;
            await StaffService.UpdateDefaultPasswordAsync(DefaultPasswordDto);
            OpenSuccessMessage(T("Set the default password successfully"));
        }
    }

    private void SetDefaultPwdAction(DefaultTextfieldAction action)
    {
        action.Content = T("Set it to the default password");
        action.OnClick = async (_) => await SetDefaultPassword(_mForm.EditContext!);
    }
}

