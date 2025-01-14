// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SSimpleSelect<TValue>
{
    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    [Parameter]
    public string? DefaultText { get; set; }

    [Parameter]
    public virtual List<(TValue value, string text)> ValueTexts { get; set; } = new();

    public string Text
    {
        get
        {
            return ValueTexts.FirstOrDefault(vt => vt.value?.Equals(Value) is true).text ?? DefaultText ?? T("Please select");
        }
        set
        {
            if (string.IsNullOrEmpty(value))
                Value = default;
        }
    }

    public bool MenuState { get; set; }

    [Parameter]
    public EventCallback OnChange { get; set; }

    private string Icon => MenuState ? "mdi-menu-up" : "mdi-menu-down";

    [Parameter]
    public bool Clearable { get; set; }

    public async Task UpdateValueAsync(TValue? value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
        if (OnChange.HasDelegate) await OnChange.InvokeAsync();
    }


}
