// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SComboBox<TItem> : STextField<string> where TItem : notnull
{
    [Parameter]
    public ICollection<TItem> Items { get; set; } = new List<TItem>();

    [Parameter]
    public RenderFragment<TItem>? ItemContent { get; set; }

    [Parameter]
    public Func<TItem, string>? ValueSelector { get; set; }

    [Parameter]
    public int MaxHeight { get; set; } = 300;

    public async Task SelectAsync(string value)
    {
        Value = value;
        if (ValueChanged.HasDelegate)
            await ValueChanged.InvokeAsync(value);
    }

    private string ConvertValue(TItem item)
    {
        if (ValueSelector is null)
            return item.ToString() ?? "";
        else return ValueSelector(item);
    }
}

