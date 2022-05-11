// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiResource;

public partial class ApiResource
{
    private string? _search;
    private int _page = 1;
    private int _pageSize = 10;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            GetApiResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetApiResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetApiResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<ApiResourceDto> ApiResources { get; set; } = new();

    public int CurrentApiResourceId { get; set; }

    public bool AddApiResourceDialogVisible { get; set; }

    public bool UpdateApiResourceDialogVisible { get; set; }

    public List<DataTableHeader<ApiResourceDto>> Headers { get; set; } = new();

    private ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("ApiResource.Name"), Value = nameof(ApiResourceDto.Name), Sortable = false },
            new() { Text = T(nameof(ApiResourceDto.DisplayName)), Value = nameof(ApiResourceDto.DisplayName), Sortable = false },
            new() { Text = T(nameof(ApiResourceDto.Description)), Value = nameof(ApiResourceDto.Description), Sortable = false },
            new() { Text = T("State"), Value = nameof(ApiResourceDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetApiResourcesAsync();
    }

    public async Task GetApiResourcesAsync()
    {
        Loading = true;
        var reuquest = new GetApiResourcesDto(Page, PageSize, Search);
        var response = await ApiResourceService.GetListAsync(reuquest);
        ApiResources = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddApiResourceDialog()
    {
        AddApiResourceDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(ApiResourceDto ApiResource)
    {
        CurrentApiResourceId = ApiResource.Id;
        UpdateApiResourceDialogVisible = true;
    }

    public async Task OpenRemoveApiResourceDialog(ApiResourceDto ApiResource)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete apiResource data"));
        if (confirm) await RemoveApiResourceAsync(ApiResource.Id);
    }

    public async Task RemoveApiResourceAsync(int ApiResourceId)
    {
        Loading = true;
        await ApiResourceService.RemoveAsync(ApiResourceId);
        OpenSuccessMessage(T("Delete apiResource data success"));
        await GetApiResourcesAsync();
        Loading = false;
    }
}

