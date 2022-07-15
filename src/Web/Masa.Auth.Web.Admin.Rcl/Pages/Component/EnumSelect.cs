// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class EnumSelect<TValue> : MSelect<KeyValuePair<string, TValue>, TValue, TValue> where TValue : struct, Enum
{
    [CascadingParameter]
    public I18n I18N { get; set; } = default!;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Clearable = true;
        Flat = true;
        Dense = true;
        Solo = true;
        HideDetails = "auto";
        await base.SetParametersAsync(parameters);
        Items = Enum.GetValues<TValue>().Select(e => new KeyValuePair<string, TValue>(e.ToString(), e)).ToList();
        ItemText = kv => I18N.T(kv.Key, true);
        ItemValue = kv => kv.Value;
    }
}

