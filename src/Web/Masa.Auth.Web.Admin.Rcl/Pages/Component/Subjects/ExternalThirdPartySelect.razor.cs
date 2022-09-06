// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ExternalThirdPartySelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public AuthenticationDefaults Value { get; set; } = new();

    [Parameter]
    public EventCallback<AuthenticationDefaults> ValueChanged { get; set; }

    public List<AuthenticationDefaults> ExternalThirdPartyIdps { get; set; } = new();

    public List<AuthenticationDefaults[]> Chunks { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        ExternalThirdPartyIdps = await AuthCaller.ThirdPartyIdpService.GetExternalThirdPartyIdpsAsync();
        Chunks = ExternalThirdPartyIdps.Chunk(11).ToList();
    }
}
