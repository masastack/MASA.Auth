// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class PlaceholderTextFiled : MTextField<string>
{
    public PlaceholderTextFiled()
    {
        HideDetails = "auto";
    }
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (!Class.StartsWith("max-auto "))
        {
            Class = "max-auto " + Class;
        }
    }
}