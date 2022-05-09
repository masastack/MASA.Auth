// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class ConfigLogin
{
    [Parameter]
    public List<CustomLoginThirdPartyIdpDto> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<CustomLoginThirdPartyIdpDto>> ValueChanged { get; set; }

    [Parameter]
    public string? Logo { get; set; }

    [Parameter]
    public string? Title { get; set; }  

    List<ThirdPartyIdpSelectDto> ThirdPartyIdps = new();

    ThirdPartyIdpService ThirdPartyIdpService => AuthCaller.ThirdPartyIdpService;

    protected override Task OnInitializedAsync()
    {
        //Toto get ThirdPartyIdps
        return base.OnInitializedAsync();
    }

    public async Task Up(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        thirdPartyIdp.Sort--;
        Value.Remove(thirdPartyIdp);
        Value.Insert(thirdPartyIdp.Sort - 1, thirdPartyIdp);
        InitSort(); 
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task Down(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        thirdPartyIdp.Sort++;
        Value.Remove(thirdPartyIdp);
        Value.Insert(thirdPartyIdp.Sort - 1, thirdPartyIdp);
        InitSort();
        await ValueChanged.InvokeAsync(Value);
    }

    public async Task Remove(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        Value.Remove(thirdPartyIdp);
        InitSort();
        await ValueChanged.InvokeAsync(Value);
    }

    public void InitSort()
    {
        int sort = 0;
        foreach (var thirdPartyIdp in Value)
        {
            thirdPartyIdp.Sort = ++sort;
        }
        Value = Value.OrderBy(v => v.Sort).ToList();
    }
}

