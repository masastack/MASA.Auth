// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultSheetDialog : SheetDialog
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        ContentClass ??= "";
        if (ContentClass.Contains("sheetDialogPadding") is false)
            ContentClass += " sheetDialogPadding";
    }

    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        if (Value)
        {
            builder.OpenElement(0, "div");
            builder.SetKey(Value);
            base.BuildRenderTree(builder);
            builder.CloseElement();
        }
    }
}
