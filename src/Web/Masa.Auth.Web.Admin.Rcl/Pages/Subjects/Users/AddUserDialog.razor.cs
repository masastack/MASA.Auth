// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Subjects.Users;

public partial class AddUserDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private int Step { get; set; } = 1;

    private bool Fill { get; set; }

    public bool Preview { get; set; }

    private AddUserDto User { get; set; } = new();

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

    protected override void OnParametersSet()
    {
        if (Visible is true)
        {
            Step = 1;
            Fill = false;
            User = new();
        }
    }

    private async Task NextStepAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            var verifySuccess = await UserService.VerifyUserRepeatAsync(new()
            {
                Account = User.Account,
                Email = User.Email,
                PhoneNumber = User.PhoneNumber,
                IdCard = User.IdCard,
            });
            if (verifySuccess)
            {
                Step = 3;
            }
        }
    }

    public async Task AddUserAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await UserService.AddAsync(User);
            OpenSuccessMessage(T("Add user data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}

