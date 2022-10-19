// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class SearchTextField : STextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    string Search { get; set; } = "";

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Flat = true;
        Solo = true;
        Small = true;
        BackgroundColor = FillBackground ? "fill-background" : "";
        Style = "max-width:340px;";
        Placeholder = I18n!.T("Search");
        PrependInnerContent = builder =>
        {
            builder.OpenComponent<MIcon>(0);
            builder.AddAttribute(1, "Size", (StringNumber)16);
            builder.AddAttribute(2, "Class", "mr-2 emphasis2--text");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
                builder2.AddContent(4, IconConstants.Search);
            });
            builder.CloseComponent();
        };

        await base.SetParametersAsync(parameters);

        var oldValueChanged = ValueChanged;
        ValueChanged = EventCallback.Factory.Create<string>(this,async value => 
        {
            Search = value;
            await Task.Delay(300);
            if (Search != value) return;
            else
            {
                await oldValueChanged.InvokeAsync(value);
            }
        });
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (Class.Contains("rounded-2 search") is false)
            Class += " rounded-2 search";
    }
}
