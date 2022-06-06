// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class SearchTextField : MTextField<string>
{
    [Inject]
    public I18n? I18n { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Flat = true;
        Dense = true;
        Solo = true;
        BackgroundColor = "fill-background";
        Style = "max-width:340px;";
        Placeholder = I18n!.T("Search");
        PrependInnerContent = builder =>
        {
            builder.OpenComponent<MIcon>(0);
            builder.AddAttribute(1, "Size", (StringNumber)16);
            builder.AddAttribute(2, "Class", "mr-2 emphasis2--text");
            builder.AddAttribute(3, "ChildContent", (RenderFragment)delegate (RenderTreeBuilder builder2) {
                builder2.AddContent(4, "mdi-magnify");//Todo: need change to IconConstants.Search
            });
            builder.CloseComponent();
        };

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (Class.Contains("rounded-2 search") is false)
            Class += " rounded-2 search";
    }
}
