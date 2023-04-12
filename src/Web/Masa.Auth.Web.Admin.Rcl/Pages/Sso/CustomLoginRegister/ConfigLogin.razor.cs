// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLoginRegister;

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

    protected override async Task OnInitializedAsync()
    {
        PageName = "CustomLoginBlock";
        ThirdPartyIdps = await ThirdPartyIdpService.GetSelectAsync();
    }

    public async Task Up(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        var oldIndex = Value.IndexOf(thirdPartyIdp);
        Value.Swap(oldIndex, oldIndex - 1);
        await InitSort();
    }

    public async Task Down(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        var oldIndex = Value.IndexOf(thirdPartyIdp);
        Value.Swap(oldIndex, oldIndex + 1);
        await InitSort();
    }

    public async Task Remove(CustomLoginThirdPartyIdpDto thirdPartyIdp)
    {
        Value.Remove(thirdPartyIdp);
        await InitSort();
    }

    public async Task InitSort()
    {
        int sort = 0;
        foreach (var thirdPartyIdp in Value)
        {
            thirdPartyIdp.Sort = ++sort;
        }
        await ValueChanged.InvokeAsync(Value);
    }
}

