// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class BooleanSelect : SSelect<KeyValuePair<string, bool?>, bool?, bool?>
{
    [CascadingParameter]
    public I18n I18N { get; set; } = default!;

    [Parameter]
    public bool FillBackground { get; set; } = true;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);
        Clearable = true;
        Items = new List<KeyValuePair<string, bool?>>()
        {
            new(I18N.T("Enable"), true),
            new(I18N.T("Disabled"), false)
        };
        ItemText = kv => kv.Key;
        ItemValue = kv => kv.Value;
        BackgroundColor = FillBackground ? "fill-background" : "white";
    }
}

