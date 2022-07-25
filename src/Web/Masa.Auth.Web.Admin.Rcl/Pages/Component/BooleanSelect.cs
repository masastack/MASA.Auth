// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class BooleanSelect: DefaultSelect<KeyValuePair<string, bool?>, bool?, bool?>
{
    [CascadingParameter]
    public I18n I18N { get; set; } = default!;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        await base.SetParametersAsync(parameters);
        Items = new List<KeyValuePair<string, bool?>>()
        {
            new(I18N.T("Enable"), true),
            new(I18N.T("Disabled"), false)
        };
        ItemText = kv => kv.Key;
        ItemValue = kv => kv.Value;
    }
}

