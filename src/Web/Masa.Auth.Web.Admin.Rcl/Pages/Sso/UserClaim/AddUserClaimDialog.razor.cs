// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.UserClaim;

public partial class AddUserClaimDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddUserClaimDto UserClaim { get; set; } = new();

    private UserClaimService UserClaimService => AuthCaller.UserClaimService;

    private MForm? Form { get; set; }

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
        if (Form is not null)
        {
            await Form.ResetValidationAsync();
        }
    }

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            UserClaim = new();
        }
    }

    public async Task AddUserClaimAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await UserClaimService.AddAsync(UserClaim);
            OpenSuccessMessage("Add userClaim success");
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
