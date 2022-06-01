// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class PlaceholderTextFiled : MTextField<string>
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        Class += " mx-auto";
        HideDetails = "auto";
    }
}
