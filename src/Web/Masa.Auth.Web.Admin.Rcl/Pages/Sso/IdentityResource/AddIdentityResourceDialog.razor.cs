// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.IdentityResource;

public partial class AddIdentityResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddIdentityResourceDto IdentityResource { get; set; } = new();

    private IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

    protected override string? PageName { get; set; } = "IdentityResourceBlock";

    private async Task UpdateVisible(bool visible)
    {
        if (!Visible)
        {
            IdentityResource = new();
        }
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    public async Task AddIdentityResourceAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await IdentityResourceService.AddAsync(IdentityResource);
            OpenSuccessMessage(T("Add identityResource success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
