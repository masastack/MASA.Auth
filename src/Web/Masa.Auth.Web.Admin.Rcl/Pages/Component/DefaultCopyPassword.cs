// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultCopyPassword : DefaultTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        Label = I18n!.T("Password");
        AppendContent = builder =>
        {
            builder.OpenComponent<PCopyableText>(0);
            builder.AddAttribute(1, "Class", "ml-n9");
            builder.AddAttribute(2, "Text", Value);
            builder.CloseComponent();
        };
        return base.SetParametersAsync(parameters);
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {        
        builder.OpenElement(0, "div");

        builder.AddAttribute(1, "class", "d-flex");
        base.BuildRenderTree(builder);

        builder.OpenComponent<MButton>(3);
        builder.AddAttribute(4, "Color", "primary");
        builder.AddAttribute(5, "Class", "ml-n2 body2");
        builder.AddAttribute(6, "Style", "width:20%;max-width:100px;height:46px;border-radius: 0px 7px 7px 0px;");
        builder.AddAttribute(7, "Onclick", EventCallback.Factory.Create<MouseEventArgs>(this, ResetPasswordAsync));
        builder.AddAttribute(8, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
            builder2.AddContent(9, I18n!.T("Reset"));
        });
        builder.CloseComponent();

        builder.CloseElement();
    }

    public async Task ResetPasswordAsync(MouseEventArgs args)
    {
        Value = RandomUtils.GenerateSpecifiedString(8, true);
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(Value);
    }
}
