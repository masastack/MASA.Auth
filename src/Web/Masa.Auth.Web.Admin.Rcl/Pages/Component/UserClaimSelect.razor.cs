// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class UserClaimSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public int Chunk { get; set; } = 5;

    [Parameter]
    public List<int> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<int>> ValueChanged { get; set; }

    IEnumerable<UserClaimSelectDto[]> UserClaimChunks { get; set; } = new List<UserClaimSelectDto[]>();

    UserClaimService UserClaimService => AuthCaller.UserClaimService;

    protected override async Task OnInitializedAsync()
    {
        var userClaims = await UserClaimService.GetSelectAsync();
        UserClaimChunks = userClaims.Chunk(Chunk);
    }

    void OnValueChanged(bool value,int id)
    {
        if(value) Value.Add(id);
        else Value.Remove(id);
    }
}

