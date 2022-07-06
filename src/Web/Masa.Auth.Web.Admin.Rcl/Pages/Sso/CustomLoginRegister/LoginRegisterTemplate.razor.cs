// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister;

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
    public string Tab { get; set; } = "";

    LoginModel Login { get; set; } = new();

    RegisterModel Register { get; set; } = new();

    protected override void OnInitialized()
    {
        PageName = "CustomLoginBlock";
        Tab ??= CustomLoginTab.Login;
    }

    protected override void OnParametersSet()
    {
        Register.RequiredFileds = RegisterFields.Where(r => r.Required).Select(r => r.RegisterFieldType.ToString()).ToList();
    }
}

