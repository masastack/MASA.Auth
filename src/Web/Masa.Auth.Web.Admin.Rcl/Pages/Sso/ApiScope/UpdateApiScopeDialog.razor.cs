// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiScope;

public partial class UpdateApiScopeDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public int ApiScopeId { get; set; }

    private ApiScopeDetailDto ApiScopeDetail { get; set; } = new();

    private UpdateApiScopeDto ApiScope { get; set; } = new();

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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetApiScopeDetailAsync();
        }
    }

    public async Task GetApiScopeDetailAsync()
    {
        ApiScopeDetail = await ApiScopeService.GetDetailAsync(ApiScopeId);
        ApiScope = ApiScopeDetail;
    }

    public async Task UpdatetApiScopeAsync(EditContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiScopeService.UpdateAsync(ApiScope);
            OpenSuccessMessage(T("Edit apiScope data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
