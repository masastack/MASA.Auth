// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public class SButton : SAutoLoadingButton
{
    [Parameter]
    public bool Medium { get; set; }

    public override async Task SetParametersAsync(ParameterView parameters)
    {
        Color = "primary";
        await base.SetParametersAsync(parameters);
    }

    protected override IEnumerable<string> BuildComponentClass()
    {
        CssBuilder cssBuilder = new CssBuilder();
        cssBuilder.Add("btn");
        if (Large)
        {
            cssBuilder.Add("large-button");
        }

        if (Medium)
        {
            cssBuilder.Add("medium-button");
        }

        if (Small)
        {
            cssBuilder.Add("small-button");
        }

        return base.BuildComponentClass().Concat(new[] { cssBuilder.ToString() });
    }
}