// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Organization;

public class PositionSelect : SComboBox<PositionSelectDto>
{
    [Inject]
    public I18n? I18n { get; set; }

    [Inject]
    public AuthCaller? AuthCaller { get; set; }

    public PositionService PositionService => AuthCaller!.PositionService;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        ValueSelector = item => item.Name;
        Label = I18n!.T("Position", true);
        MaxHeight = 250;
        await base.SetParametersAsync(parameters);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();
        PositionService.GetSelectAsync().ContinueWith(async task =>
        {
            Items = task.Result;
            await InvokeAsync(StateHasChanged);
        });
    }
}
