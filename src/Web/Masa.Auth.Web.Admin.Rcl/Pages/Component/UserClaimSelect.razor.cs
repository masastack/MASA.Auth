// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class UserClaimSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public List<int> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<int>> ValueChanged { get; set; }
}

