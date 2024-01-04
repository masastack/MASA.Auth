// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class ButtonGroup<TValue> where TValue : struct, Enum
{
    [Parameter]
    public TValue Value { get; set; }

    [Parameter]
    public EventCallback<TValue> ValueChanged { get; set; }

    [Parameter]
    public StyleTypes StyleType { get; set; }

    public List<KeyValuePair<string, TValue>> KeyValues { get; set; } = new();

    protected override void OnInitialized()
    {
        KeyValues = GetEnumMap<TValue>();
    }
}

