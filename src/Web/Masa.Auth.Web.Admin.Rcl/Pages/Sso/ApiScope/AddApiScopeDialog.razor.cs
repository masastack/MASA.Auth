// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiScope;

public partial class AddApiScopeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddApiScopeDto ApiScope { get; set; } = new();

    private ApiScopeService ApiScopeService => AuthCaller.ApiScopeService;

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

    protected override void OnParametersSet()
    {
        if (Visible)
        {
            ApiScope = new();
        }
    }

    public async Task AddApiScopeAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiScopeService.AddAsync(ApiScope);
            OpenSuccessMessage(T("Add apiScope success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
