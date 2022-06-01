// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using Microsoft.AspNetCore.Components.Rendering;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultTest : ADialog
{
   

    protected override void BuildRenderTree(RenderTreeBuilder __builder)
    {
        base.BuildRenderTree(__builder);
        __builder.OpenRegion(18);
        __builder.OpenElement(0, "div");
        __builder.AddAttribute(1, "class", "mt-3 hover-pointer font-16");

        __builder.OpenComponent<MIcon>(2);
        __builder.AddAttribute(3, "Size", 18);
        __builder.AddMarkupContent(4, "mdi-autorenew");//need change to IconConstants.Refresh
        __builder.CloseComponent();

        __builder.CloseElement();
        __builder.CloseRegion();
    }
}
