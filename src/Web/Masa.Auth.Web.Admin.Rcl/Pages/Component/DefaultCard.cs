// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public class DefaultCard : MCard
{
    protected override void OnParametersSet()
    {
        base.OnParametersSet();
        Class ??= "";
        Class += " pa-6 full-height";
    }
}
