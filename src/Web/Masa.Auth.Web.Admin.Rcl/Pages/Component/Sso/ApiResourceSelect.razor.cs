// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Sso;

public partial class ApiResourceSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public int Chunk { get; set; } = 5;

    [Parameter]
    public List<Guid> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<Guid>> ValueChanged { get; set; }

    IEnumerable<ApiResourceSelectDto[]> ApiResourceChunks { get; set; } = new List<ApiResourceSelectDto[]>();

    ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

    protected override async Task OnInitializedAsync()
    {
        var apiResources = await ApiResourceService.GetSelectAsync();
        ApiResourceChunks = apiResources.Chunk(Chunk);
    }

    void OnValueChanged(bool value, Guid id)
    {
        if (value) Value.Add(id);
        else Value.Remove(id);
    }
}

