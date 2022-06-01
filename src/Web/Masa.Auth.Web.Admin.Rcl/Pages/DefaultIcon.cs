// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Stack.Components;

public class DefaultIcon : MIcon
{
    [Parameter]
    public bool Add { get; set; }

    [Parameter]
    public bool Update { get; set; }

    [Parameter]
    public bool Remove { get; set; }

    [Parameter]
    public bool Authorize { get; set; }

    protected override void OnInitialized()
    {
        if ((Small, Large, XLarge, XSmall) == (false, false, false, false)) Small = true;
        if (Color is null) Color = "default-regular";
        string icon = "";
        if (Add) icon = "mdi-plus";
        else if (Update) icon = "mdi-pencil";
        else if (Remove) icon = "mdi-delete";
        else if (Authorize) icon = "mdi-shield-half-full";

        ChildContent = (builder) =>
        {
            builder.AddMarkupContent(0, icon);
        };
        base.OnInitialized();
    }
}