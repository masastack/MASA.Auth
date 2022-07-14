// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class EnumAutocomplete<TValue> : MAutocomplete<KeyValuePair<string, TValue>, TValue, TValue> where TValue : struct, Enum
{
    [CascadingParameter]
    public I18n I18N { get; set; } = default!;

    [Parameter]
    public bool FillBackground { get; set; } = true;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Flat = true;
        Dense = true;
        Solo = true;
        HideDetails = "auto";
        Color = "primary";
        Style = "";
        Class = "";
        HideDetails = "auto";
        await base.SetParametersAsync(parameters);
        Items = Enum.GetValues<TValue>().Select(e => new KeyValuePair<string, TValue>(e.ToString(), e)).ToList();
        ItemText = kv => I18N.T(kv.Key, true);
        ItemValue = kv => kv.Value;
        BackgroundColor = FillBackground ? "fill-background" : "";
    }

    protected override void OnParametersSet()
    {
        if (Style.Contains("width:340px;height:40px;") is false) Style += "width:340px;height:40px;";
        if (Class.Contains("rounded-2") is false) Class += " rounded-2";
    }
}
