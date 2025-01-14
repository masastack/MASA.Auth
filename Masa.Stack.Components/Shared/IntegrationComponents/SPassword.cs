// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public class SPassword : MTextField<string>
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

    protected override void OnParametersSet()
    {
        Label ??= I18n?.T("Password", true);
        base.OnParametersSet();
    }

    public SPassword()
    {
        Type = "password";
        AppendIcon = "mdi-eye-off";
    }

    private void SwitchPassword()
    {
        ShowPassword = !ShowPassword;
        Type = ShowPassword ? "text" : "password";
        AppendIcon = ShowPassword ? "mdi-eye" : "mdi-eye-off";
    }
}

