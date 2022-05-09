// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class RegisterFieldSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public RegisterFieldTypes Value { get; set; } = new();

    [Parameter]
    public EventCallback<RegisterFieldTypes> ValueChanged { get; set; }

    [Parameter]
    public List<RegisterFieldTypes> RegisterFields { get; set; } = new();

    [Parameter]
    public List<RegisterFieldTypes> Excludes { get; set; } = new();

    ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override void OnInitialized()
    {
        RegisterFields = Enum.GetValues<RegisterFieldTypes>().ToList();
    }
}

