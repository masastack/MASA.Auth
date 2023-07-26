// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiResource;

public partial class ApiResource
{
    private string? _search;
    private int _page = 1, _pageSize = 20;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
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
            _page = 1;
            _pageSize = value;
            GetApiResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<ApiResourceDto> ApiResources { get; set; } = new();

    public Guid CurrentApiResourceId { get; set; }

    public bool AddApiResourceDialogVisible { get; set; }

    public bool UpdateApiResourceDialogVisible { get; set; }

    private ApiResourceService ApiResourceService => AuthCaller.ApiResourceService;

    protected override async Task OnInitializedAsync()
    {
        PageName = "ApiResourceBlock";
        await GetApiResourcesAsync();
    }

    public List<DataTableHeader<ApiResourceDto>> GetHeaders() => new()
    {
        new() { Text = T("Name"), Value = nameof(ApiResourceDto.Name), Sortable = false, Width="250px" },
        new() { Text = T(nameof(ApiResourceDto.DisplayName)), Value = nameof(ApiResourceDto.DisplayName), Sortable = false, Width="250px" },
        new() { Text = T(nameof(ApiResourceDto.Description)), Value = nameof(ApiResourceDto.Description), Sortable = false },
        new() { Text = T("State"), Value = nameof(ApiResourceDto.Enabled), Sortable = false , Width="105px"},
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetApiResourcesAsync()
    {
        var reuquest = new GetApiResourcesDto(Page, PageSize, Search);
        var response = await ApiResourceService.GetListAsync(reuquest);
        ApiResources = response.Items;
        Total = response.Total;
    }

    public void OpenAddApiResourceDialog()
    {
        AddApiResourceDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(ApiResourceDto apiResource)
    {
        CurrentApiResourceId = apiResource.Id;
        UpdateApiResourceDialogVisible = true;
    }

    public async Task OpenRemoveApiResourceDialog(ApiResourceDto apiResource)
    {
        var confirm = await OpenConfirmDialog(T("Delete ApiResource"), T("Are you sure delete apiResource \"{0}\"?", apiResource.Name));
        if (confirm) await RemoveApiResourceAsync(apiResource.Id);
    }

    public async Task RemoveApiResourceAsync(Guid apiResourceId)
    {
        await ApiResourceService.RemoveAsync(apiResourceId);
        OpenSuccessMessage(T("Delete apiResource data success"));
        await GetApiResourcesAsync();
    }
}

