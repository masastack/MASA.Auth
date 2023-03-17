// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class PlaceholderTextField : MTextField<string>
{
    public PlaceholderTextField()
    {
        HideDetails = "auto";
    }
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (!Class.StartsWith("mx-auto "))
        {
            Class = "mx-auto " + Class;
        }
        Style ??= "";
        if(!Style.StartsWith("color: #A3AED0 !important;"))
        {
            Style = "color: #A3AED0 !important;font-size: 24px !important;" + Style;
        }
    }
}