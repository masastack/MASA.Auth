// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class DefaultCopyPassword : DefaultTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    public bool Visible { get; set; }

    public new string Type { get; set; } = "password";

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = I18n!.T("User", "Password");
        Readonly = true;
        base.Type = Type;
        AppendContent = builder =>
        {
            if (Type == "text")
            {
                builder.OpenComponent<PCopyableText>(0);
                builder.AddAttribute(1, "Class", "ml-n9");
                builder.AddAttribute(2, "Text", Value);
                builder.CloseComponent();
            }
        };
        return base.SetParametersAsync(parameters);
    }

    public async Task ResetPasswordAsync()
    {
        Type = "text";
        var value = RandomUtils.GenerateSpecifiedString(8, true);
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
