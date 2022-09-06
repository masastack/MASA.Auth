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
    public string Value { get; set; } = "";

    private string InternalValue { get; set; } = "";

    [Parameter]
    public EventCallback<AuthenticationDefaults> ValueChanged { get; set; }

    public List<AuthenticationDefaults> ExternalThirdPartyIdps { get; set; } = new();

    public List<AuthenticationDefaults[]> Chunks { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        ExternalThirdPartyIdps = await AuthCaller.ThirdPartyIdpService.GetExternalThirdPartyIdpsAsync();
        ExternalThirdPartyIdps.Add(new AuthenticationDefaults
        {
            Scheme = ThirdPartyIdpTypes.Customize.ToString()
        });
        Chunks = ExternalThirdPartyIdps.Chunk(11).ToList();
    }

    protected override void OnParametersSet()
    {
        if(InternalValue != Value)
        {
            InternalValue = Value;
            var value = ExternalThirdPartyIdps.FirstOrDefault(v => v.Scheme == InternalValue);
            if(value != null)
            {
                var middle = ((ExternalThirdPartyIdps.Count <= 11 ? ExternalThirdPartyIdps.Count : 11) + 1) / 2;
                ExternalThirdPartyIdps.Remove(value);
                ExternalThirdPartyIdps.Insert(middle -1, value);
                Chunks = ExternalThirdPartyIdps.Chunk(11).ToList();
            }
        }      
    }

    public async Task UpdateValueAsync(AuthenticationDefaults value)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else Value = value.Scheme;
    }
}
