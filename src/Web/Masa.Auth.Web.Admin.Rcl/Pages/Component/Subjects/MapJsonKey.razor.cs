// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Subjects;

public partial class MapJsonKey
{
    [Parameter]
    public List<KeyValue> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<KeyValue>> ValueChanged { get; set; }

    Dictionary<string, string> UserClaims { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        UserClaims = await AuthCaller.ThirdPartyIdpService.GetUserClaims();
    }

    public async Task RemoveAsync(KeyValue keyVlaue)
    {
        var value = new List<KeyValue>(Value);
        value.Remove(keyVlaue);
        await UpdateValueAsync(value);
    }

    public async Task AddAsync()
    {
        var value = new List<KeyValue>(Value);
        value.Add(new());
        await UpdateValueAsync(value);
    }

    public async Task UpdateValueAsync(List<KeyValue> value)
    {
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
