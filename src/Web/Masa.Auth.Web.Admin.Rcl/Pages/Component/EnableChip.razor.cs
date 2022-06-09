// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class EnableChip
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public bool Value { get; set; }

    public string TextColor => Value ? "green" : "error";

    public string Color => Value ? "green-lighten" : "warning-lighten";
}

