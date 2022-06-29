// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.IdentityResource;

public partial class IdentityResource
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
            GetIdentityResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetIdentityResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetIdentityResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<IdentityResourceDto> IdentityResources { get; set; } = new();

    public int CurrentIdentityResourceId { get; set; }

    public bool AddIdentityResourceDialogVisible { get; set; }

    public bool UpdateIdentityResourceDialogVisible { get; set; }

    public List<DataTableHeader<IdentityResourceDto>> Headers { get; set; } = new();

    private IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T("IdentityResource.Name"), Value = nameof(IdentityResourceDto.Name), Sortable = false },
            new() { Text = T(nameof(IdentityResourceDto.DisplayName)), Value = nameof(IdentityResourceDto.DisplayName), Sortable = false },
            new() { Text = T("IdentityResource.Required"), Value = nameof(IdentityResourceDto.Required), Sortable = false },
            new() { Text = T(nameof(IdentityResourceDto.Description)), Value = nameof(IdentityResourceDto.Description), Sortable = false },
            new() { Text = T("State"), Value = nameof(IdentityResourceDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align="center", Width="105px" },
        };

        await GetIdentityResourcesAsync();
    }

    public async Task GetIdentityResourcesAsync()
    {
        Loading = true;
        var reuquest = new GetIdentityResourcesDto(Page, PageSize, Search);
        var response = await IdentityResourceService.GetListAsync(reuquest);
        IdentityResources = response.Items;
        Total = response.Total;
        Loading = false;
    }

    private async Task AddStandardIdentityResourcesAsync()
    {
        Loading = true;
        await IdentityResourceService.AddStandardIdentityResourcesAsync();
        OpenSuccessMessage(T("Add standard identityResource data success"));
        await GetIdentityResourcesAsync();
        Loading = false;
    }

    public void OpenAddRoleDialog()
    {
        AddIdentityResourceDialogVisible = true;
    }

    public void OpenUpdateRoleDialog(IdentityResourceDto identityResource)
    {
        CurrentIdentityResourceId = identityResource.Id;
        UpdateIdentityResourceDialogVisible = true;
    }

    public async Task OpenRemoveIdentityResourceDialog(IdentityResourceDto identityResource)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete identityResource data"));
        if (confirm) await RemoveIdentityResourceAsync(identityResource.Id);
    }

    public async Task RemoveIdentityResourceAsync(int identityResourceId)
    {
        Loading = true;
        await IdentityResourceService.RemoveAsync(identityResourceId);
        OpenSuccessMessage(T("Delete identityResource data success"));
        await GetIdentityResourcesAsync();
        Loading = false;
    }
}

