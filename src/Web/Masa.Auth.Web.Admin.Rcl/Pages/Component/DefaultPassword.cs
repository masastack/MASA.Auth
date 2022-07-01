// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultPassword : MTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    bool ShowPassword { get; set; }

    protected override void OnInitialized()
    {
        HideDetails = "auto";
        Outlined = true;
        OnAppendClick = EventCallback.Factory.Create<MouseEventArgs>(this, SwitchPassword);
        base.OnInitialized();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label ??= I18n?.T("Password", true);
        Type = ShowPassword ? "text" : "password";
        AppendIcon = ShowPassword ? "mdi-eye" : "mdi-eye-off";
        return base.SetParametersAsync(parameters);
    }

    private void SwitchPassword() => ShowPassword = !ShowPassword;
}

