// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class RegisterFieldSelect
{
    [Parameter]
    public List<RegisterFieldDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<RegisterFieldDto>> ValueChanged { get; set; }
}

