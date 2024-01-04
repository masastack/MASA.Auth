// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Sso;

public partial class IdentityResourceSelect
{
    [Parameter]
    public int Chunk { get; set; } = 5;

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    IEnumerable<IdentityResourceSelectDto[]> IdentityResourceChunks { get; set; } = new List<IdentityResourceSelectDto[]>();

    IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

    protected override async Task OnInitializedAsync()
    {
        var identityResources = await IdentityResourceService.GetSelectAsync();
        IdentityResourceChunks = identityResources.Chunk(Chunk);
    }

    void OnValueChanged(bool value, Guid id)
    {
        if (value)
            Value.Add(id);
        else
            Value.Remove(id);
    }
}

