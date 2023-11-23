// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Masa.Auth.Contracts.Admin.Sso;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiResource;

public partial class UpdateApiResourceDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSubmitSuccess { get; set; }

    [Parameter]
    public Guid ApiResourceId { get; set; }

    private ApiResourceDetailDto ApiResourceDetail { get; set; } = new();

    private UpdateApiResourceDto ApiResource { get; set; } = new();

    private ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

    Type _otherType = null!;
    Dictionary<string, object> _componentMetadata = new();
    ClientCredentialDto _clientCredentialDto = new();

    protected override void OnInitialized()
    {
        PageName = "ApiResourceBlock";
        PrepareHeader();
        base.OnInitialized();
    }

    private void PrepareHeader()
    {

        _otherType = typeof(ClientSecret);
        _componentMetadata = new Dictionary<string, object>{
                { "Dto",_clientCredentialDto }
            };
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

    protected override async Task OnParametersSetAsync()
    {
        if (Visible)
        {
            await GetApiResourceDetailAsync();
        }
    }

    public async Task GetApiResourceDetailAsync()
    {
        ApiResourceDetail = await ApiResourceService.GetDetailAsync(ApiResourceId);
        ApiResource = ApiResourceDetail;
    }

    public async Task UpdatetApiResourceAsync(FormContext context)
    {
        var success = context.Validate();
        if (success)
        {
            await ApiResourceService.UpdateAsync(ApiResource);
            OpenSuccessMessage(T("Edit apiResource data success"));
            await UpdateVisible(false);
            await OnSubmitSuccess.InvokeAsync();
        }
    }
}
