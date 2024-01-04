// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Sso;

public partial class UserClaimSelect
{
    [Parameter]
    public int Chunk { get; set; } = 4;

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    IEnumerable<UserClaimSelectDto[]> UserClaimChunks { get; set; } = new List<UserClaimSelectDto[]>();

    UserClaimService UserClaimService => AuthCaller.UserClaimService;

    protected override async Task OnInitializedAsync()
    {
        var userClaims = await UserClaimService.GetSelectAsync();
        UserClaimChunks = userClaims.Chunk(Chunk);
    }

    void OnValueChanged(bool value, Guid id)
    {
        if (value) Value.Add(id);
        else Value.Remove(id);
    }
}

