// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultButton : MButton
{
    [Parameter]
    public bool Medium { get; set; }

    protected override void SetComponentClass()
    {
        base.SetComponentClass();

        CssProvider.Merge(delegate (CssBuilder cssBuilder) {
            cssBuilder.AddFirstIf(("large-button", () => Large), ("medium-button", () => Medium), ("small-button", () => Small));
        });
    }
}