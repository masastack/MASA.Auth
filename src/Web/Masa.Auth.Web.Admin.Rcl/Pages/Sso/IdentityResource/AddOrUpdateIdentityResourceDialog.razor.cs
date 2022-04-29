// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.IdentityResource;

public partial class AddOrUpdateIdentityResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int IdentityResourceId { get; set; }

    public bool IsAdd => IdentityResourceId == 0;

    private IdentityResourceDetailDto IdentityResourceDetail { get; set; } = new();

    private IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible && IsAdd is false)
        {
            await GetIdentityResourceDetailAsync();
        }
    }

    public async Task GetIdentityResourceDetailAsync()
    {
        IdentityResourceDetail = await IdentityResourceService.GetDetailAsync(IdentityResourceId);
    }

    public async Task AddOrUpdatetIdentityResourceAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            if (IsAdd)
            {
                await IdentityResourceService.AddAsync(IdentityResourceDetail);
                OpenSuccessMessage("Add identityResource success");
            }
            else
            {
                await IdentityResourceService.UpdateAsync(IdentityResourceDetail);
                OpenSuccessMessage("Update identityResource success");
            }
            await OnSubmitSuccess.InvokeAsync();
            await UpdateVisible(false);
            Loading = false;
        }
    }
}
