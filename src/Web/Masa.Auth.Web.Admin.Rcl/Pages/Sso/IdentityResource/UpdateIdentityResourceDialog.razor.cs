// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.IdentityResource;

public partial class UpdateIdentityResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int IdentityResourceId { get; set; }

    private IdentityResourceDetailDto IdentityResourceDetail { get; set; } = new();

    private UpdateIdentityResourceDto IdentityResource { get; set; } = new();

    private IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

    private MForm? Form { get; set; }

    protected override string? PageName { get; set; } = "IdentityResourceBlock";

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
        if (Visible)
        {
            await GetIdentityResourceDetailAsync();
        }
    }

    public async Task GetIdentityResourceDetailAsync()
    {
        IdentityResourceDetail = await IdentityResourceService.GetDetailAsync(IdentityResourceId);
        IdentityResource = IdentityResourceDetail;
    }

    public async Task UpdatetIdentityResourceAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await IdentityResourceService.UpdateAsync(IdentityResource);
            OpenSuccessMessage("Update identityResource success");
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
