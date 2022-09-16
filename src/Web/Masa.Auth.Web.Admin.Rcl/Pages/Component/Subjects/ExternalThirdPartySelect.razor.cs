// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ExternalThirdPartySelect
{
    bool _expand;

    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public string Value { get; set; } = "";

    private string InternalValue { get; set; } = "";

    [Parameter]
    public EventCallback<ThirdPartyIdpModel> ValueChanged { get; set; }

    public List<ThirdPartyIdpModel> ExternalThirdPartyIdps { get; set; } = new();

    public List<ThirdPartyIdpModel[]> Chunks { get; set; } = new();

    public bool Expand
    {
        get => _expand || WaitUpload;
        set 
        {
            _expand = value;
        }
    }

    public bool WaitUpload { get; set; }

    public bool Even { get; set; }

    protected override async Task OnInitializedAsync()
    {
        ExternalThirdPartyIdps = await AuthCaller.ThirdPartyIdpService.GetExternalThirdPartyIdpsAsync();
        ExternalThirdPartyIdps.Add(new ThirdPartyIdpModel
        {
            Name = ThirdPartyIdpTypes.Customize.ToString()
        });
        Chunks = ExternalThirdPartyIdps.Chunk(11).ToList();
        Even = ExternalThirdPartyIdps.Count % 2 == 0;
    }

    protected override void OnParametersSet()
    {
        if (InternalValue != Value && ExternalThirdPartyIdps.Count > 0)
        {
            InternalValue = Value;
            var value = ExternalThirdPartyIdps.FirstOrDefault(v => v.Name == InternalValue);
            if (value != null)
            {
                var middle = ((ExternalThirdPartyIdps.Count <= 11 ? ExternalThirdPartyIdps.Count : 11) + 1) / 2;
                ExternalThirdPartyIdps.Remove(value);
                ExternalThirdPartyIdps.Insert(middle - 1, value);
                Chunks = ExternalThirdPartyIdps.Chunk(11).ToList();
            }
            Expand = false;            
        }
        WaitUpload = false;
    }

    public async Task UpdateValueAsync(ThirdPartyIdpModel value)
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(value);
        }
        else Value = value.Name;       
    }

    public void OnUploadChang()
    {
        WaitUpload = true;
    }
}
