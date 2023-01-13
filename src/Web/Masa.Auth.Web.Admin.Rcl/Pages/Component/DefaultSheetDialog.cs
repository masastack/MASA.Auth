// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultSheetDialog : SSheetDialog
{
    bool? _oldValue;

    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        EnableDomReload = true;
        ContentClass ??= "";
        if (ContentClass.Contains("sheetDialogPadding") is false)
            ContentClass += " sheetDialogPadding";
    }
}
