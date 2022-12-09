// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ThirdPartyIdpSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public Guid Value { get; set; } = new();

    [Parameter]
    public EventCallback<Guid> ValueChanged { get; set; }

    [Parameter]
    public List<ThirdPartyIdpSelectDto>? ThirdPartyIdps { get; set; }

    [Parameter]
    public List<Guid> Excludes { get; set; } = new();

    [Parameter]
    public bool Small { get; set; }

    [Parameter]
    public string? Label { get; set; }

    [Parameter]
    public bool Clearable { get; set; }

    [Parameter]
    public bool FillBackground { get; set; } = true;

    ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override async Task OnInitializedAsync()
    {
        if (ThirdPartyIdps is null)
        {
            await InitThirdPartyIdps();
        }
        else Label ??= "ThirdPartyIdp";
    }

    public async Task InitThirdPartyIdps()
    {
        ThirdPartyIdps = new();
        ThirdPartyIdps.AddRange(await ThirdPartyIdpService.GetSelectAsync(default, true));
    }
}

