// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class ExternalThirdPartySelect
{
    [Parameter]
    public string Value { get; set; } = "";

    private string InternalValue { get; set; } = "";

    [Parameter]
    public EventCallback<ThirdPartyIdpModel> ValueChanged { get; set; }

    public List<ThirdPartyIdpModel> ExternalThirdPartyIdps { get; set; } = new();

    public bool Expand { get; set; }

    public bool WaitUpload { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            ExternalThirdPartyIdps = await AuthCaller.ThirdPartyIdpService.GetExternalThirdPartyIdpsAsync();
            //ExternalThirdPartyIdps.Add(new ThirdPartyIdpModel
            //{
            //    Name = ThirdPartyIdpTypes.Customize.ToString()
            //});
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    protected override void OnParametersSet()
    {
        if (InternalValue != Value && ExternalThirdPartyIdps.Count > 0)
        {
            InternalValue = Value;
            var value = ExternalThirdPartyIdps.FirstOrDefault(v => v.Name == InternalValue);
            if (value != null)
            {
                var middle = ExternalThirdPartyIdps.Count / 2;
                ExternalThirdPartyIdps.Remove(value);
                ExternalThirdPartyIdps.Insert(middle, value);
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
