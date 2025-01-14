// Copyright (c) MASA Stack All rights reserved.
// Licensed under the MIT License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public partial class SDropdown<TItem, TValue>
{
    [Parameter]
    public RenderFragment? ChildContent { get; set; }

    [Parameter]
    public RenderFragment<ActivatorProps>? ActivatorContent { get; set; }

    [Parameter, EditorRequired]
    public List<TItem> Items { get; set; } = new();

    [Parameter, EditorRequired]
    public Func<TItem, TValue> ItemValue { get; set; } = null!;

    [Parameter, EditorRequired]
    public Func<TItem, string> ItemText { get; set; } = null!;

    [Parameter]
    public Func<TItem, bool> ItemDisabled { get; set; } = _ => false;

    [Parameter]
    public Func<TItem, string>? ItemIcon { get; set; }

    [Parameter]
    public RenderFragment<TItem>? ItemTemplate { get; set; }

    [Parameter]
    public RenderFragment? PrependContent { get; set; }

    [Parameter]
    public EventCallback<TItem> OnItemClick { get; set; }

    [Parameter]
    public TValue? Value { get; set; }

    [Parameter]
    public EventCallback<TValue?> ValueChanged { get; set; }

    [Parameter(CaptureUnmatchedValues = true)]
    public Dictionary<string, object> Attributes { get; set; } = new();

    private StringNumber _itemValue = 0;

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        await base.SetParametersAsync(parameters);

        ArgumentNullException.ThrowIfNull(Items);
        ArgumentNullException.ThrowIfNull(ItemValue);
        ArgumentNullException.ThrowIfNull(ItemText);
    }

    protected override void OnInitialized()
    {
        base.OnInitialized();

        var defaultSelectedIndex = Items.FindIndex(item =>
        {
            var val = ItemValue.Invoke(item);
            if (val == null && Value == null)
            {
                return true;
            }

            return val!.Equals(Value);
        });
        _itemValue = defaultSelectedIndex;
    }

    private async Task ItemValueChanged(StringNumber val)
    {
        _itemValue = val;

        var index = val.ToInt32();

        var item = Items.ElementAt(index);

        var value = ItemValue.Invoke(item);

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else
        {
            Value = value;
        }
    }

    private async Task HandleOnItemClick(TItem item)
    {
        if (OnItemClick.HasDelegate)
        {
            await OnItemClick.InvokeAsync(item);
        }
    }
}
