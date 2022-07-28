// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.ApiScope;

public partial class ApiScope
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
            _page = 1;
            GetApiScopesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetApiScopesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetApiScopesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<ApiScopeDto> ApiScopes { get; set; } = new();

    public int CurrentApiScopeId { get; set; }

    public bool AddApiScopeDialogVisible { get; set; }

    public bool UpdateApiScopeDialogVisible { get; set; }

    private ApiScopeService ApiScopeService => AuthCaller.ApiScopeService;

    protected override async Task OnInitializedAsync()
    {
        await GetApiScopesAsync();
    }

    public List<DataTableHeader<ApiScopeDto>> GetHeaders() => new()
    {
        new() { Text = T("ApiScope.Name"), Value = nameof(ApiScopeDto.Name), Sortable = false },
        new() { Text = T(nameof(ApiScopeDto.DisplayName)), Value = nameof(ApiScopeDto.DisplayName), Sortable = false },
        new() { Text = T("ApiScope.Required"), Value = nameof(ApiScopeDto.Required), Sortable = false },
        new() { Text = T(nameof(ApiScopeDto.Description)), Value = nameof(ApiScopeDto.Description), Sortable = false },
        new() { Text = T("State"), Value = nameof(ApiScopeDto.Enabled), Sortable = false },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align="center", Width="105px" },
    };

    public async Task GetApiScopesAsync()
    {
        Loading = true;
        var reuquest = new GetApiScopesDto(Page, PageSize, Search);
        var response = await ApiScopeService.GetListAsync(reuquest);
        ApiScopes = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddApiResourceDialog()
    {
        AddApiScopeDialogVisible = true;
    }

    public void OpenUpdateApiResourceDialog(ApiScopeDto ApiScope)
    {
        CurrentApiScopeId = ApiScope.Id;
        UpdateApiScopeDialogVisible = true;
    }

    public async Task OpenRemoveApiScopeDialog(ApiScopeDto ApiScope)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete apiScope data"));
        if (confirm) await RemoveApiScopeAsync(ApiScope.Id);
    }

    public async Task RemoveApiScopeAsync(int ApiScopeId)
    {
        Loading = true;
        await ApiScopeService.RemoveAsync(ApiScopeId);
        OpenSuccessMessage(T("Delete apiScope data success"));
        await GetApiScopesAsync();
        Loading = false;
    }
}

