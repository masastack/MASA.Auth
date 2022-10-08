// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.UserClaim;

public partial class UpdateUserClaimDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int UserClaimId { get; set; }

    private UserClaimDetailDto UserClaimDetail { get; set; } = new();

    private UpdateUserClaimDto UserClaim { get; set; } = new();

    private UserClaimService UserClaimService => AuthCaller.UserClaimService;

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
            await GetUserClaimDetailAsync();
        }
    }

    public async Task GetUserClaimDetailAsync()
    {
        UserClaimDetail = await UserClaimService.GetDetailAsync(UserClaimId);
        UserClaim = UserClaimDetail;
    }

    public async Task UpdatetUserClaimAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await UserClaimService.UpdateAsync(UserClaim);
            OpenSuccessMessage(T("Edit userClaim data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
