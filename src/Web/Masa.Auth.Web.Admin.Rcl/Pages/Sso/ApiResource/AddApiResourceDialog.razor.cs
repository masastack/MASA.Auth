// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiResource;

public partial class AddApiResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    private AddApiResourceDto ApiResource { get; set; } = new();

    private ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

    protected override void OnInitialized()
    {
        PageName = "ApiResourceBlock";
        base.OnInitialized();
    }

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
        if (!Visible)
        {
            ApiResource = new();
        }
    }

    public async Task AddApiResourceAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            Loading = true;
            await ApiResourceService.AddAsync(ApiResource);
            OpenSuccessMessage(T("Add apiResource success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
            Loading = false;
        }
    }
}
