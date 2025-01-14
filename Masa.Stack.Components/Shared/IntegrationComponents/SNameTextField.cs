// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class SNameTextField : MTextField<string>
{
    public SNameTextField()
    {
        HideDetails = "auto";
    }
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        if (!Class.StartsWith("name-textfield "))
        {
            Class = "name-textfield " + Class;
        }
    }
}
