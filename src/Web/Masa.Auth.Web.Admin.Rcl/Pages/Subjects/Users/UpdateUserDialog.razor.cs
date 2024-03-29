﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class UpdateUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid UserId { get; set; }

    [CascadingParameter]
    public Task<AuthenticationState> AuthenticationStateTask { get; set; } = default!;

    [Inject]
    public IPermissionValidator Validator { get; set; } = default!;

    private bool CanUpdateAccount { get; set; }

    private UserDetailDto UserDetail { get; set; } = new();

    private UpdateUserDto User { get; set; } = new();

    private ResetUserPasswordDto UserPassword = new();

    private UserService UserService => AuthCaller.UserService;

    protected override string? PageName { get; set; } = "UserBlock";

    private async Task UpdateVisible(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = false;
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var state = await AuthenticationStateTask;
            CanUpdateAccount = !Validator.Validate(ElementPermissionCodes.UserAccountUpdate, state.User);
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetUserDetailAsync();
        }
    }

    public async Task GetUserDetailAsync()
    {
        UserDetail = await UserService.GetDetailAsync(UserId);
        User = UserDetail;
        UserPassword = new ResetUserPasswordDto(UserDetail.Id, UserDetail.Password);
    }

    public async Task UpdateUserAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            await UserService.UpdateAsync(User);
            OpenSuccessMessage(T("Edit user data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
        }
    }

    public async Task ResetUserPasswordAsync(string password)
    {
        UserPassword.Password = password;
        await UserService.ResetPasswordAsync(UserPassword);
        OpenSuccessMessage(T("Password changed successfully"));
    }

    public async Task OpenRemoveUserDialog()
    {
        var confirm = await OpenConfirmDialog(T("Delete User"), T("Are you sure delete user \"{0}\"?", User.DisplayName));
        if (confirm) await RemoveUserAsync();
    }

    public async Task RemoveUserAsync()
    {
        await UserService.RemoveAsync(UserId);
        OpenSuccessMessage(T("Delete user data success"));
        await UpdateVisible(false);
        await OnSubmitSuccess.InvokeAsync();
    }
}

