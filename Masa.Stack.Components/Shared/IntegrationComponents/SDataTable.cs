// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public class SDataTable<TItem> : MDataTable<TItem>
{
    [Parameter]
    public bool DisableChippedEnum { get; set; }

    [Parameter]
    public string Theme { get; set; } = "table-border-head";

    public SDataTable()
    {
        HideDefaultFooter = true;
        FixedHeader = true;
    }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        ItemColContent = item =>
        {
            void Content(RenderTreeBuilder builder)
            {
                builder.OpenComponent(0, typeof(SItemCol));
                builder.AddAttribute(1, nameof(SItemCol.Value), item.Value);
                builder.AddAttribute(2, nameof(SItemCol.ChippedEnum), !DisableChippedEnum);
                builder.AddAttribute(3, nameof(SItemCol.SmallChip), Dense);
                builder.CloseComponent();
            }

            return Content;
        };

        await base.SetParametersAsync(parameters);
    }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (Class.Contains(Theme) is false)
            Class += $" {Theme}";
        if (Dense && Class.Contains("dense") is false)
            Class += " dense";
    }
}
