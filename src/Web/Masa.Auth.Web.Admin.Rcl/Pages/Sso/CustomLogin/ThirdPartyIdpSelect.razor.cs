// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class ThirdPartyIdpSelect
{
    [Parameter]
    public List<CustomLoginThirdPartyIdpDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<CustomLoginThirdPartyIdpDto>> ValueChanged { get; set; }

    StringNumber LoginTab { get; set; } = CustomLoginTab.Login;
}

