// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class LoginRegisterTemplate
{
    [Parameter]
    public string? Logo { get; set; }

    [Parameter]
    public string? Title { get; set; }

    [Parameter]
    public List<ThirdPartyIdpSelectDto> ThirdPartyIdps { get; set; } = new();

    [Parameter]
    public List<RegisterFieldDto> RegisterFields { get; set; } = new();

    [Parameter]
    public StringNumber Tab { get; set; } = CustomLoginTab.Login;

    LoginModel Login { get; set; } = new();

    RegisterModel Register { get; set; } = new ();

    protected override void OnParametersSet()
    {
        Login = new();
    }
}

