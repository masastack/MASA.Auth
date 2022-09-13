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

    public async Task UpdatetIdentityResourceAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await IdentityResourceService.UpdateAsync(IdentityResource);
            OpenSuccessMessage(T("Edit identityResource data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
