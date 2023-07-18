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
            _page = 1;
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
            _page = 1;
            _pageSize = value;
            GetIdentityResourcesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<IdentityResourceDto> IdentityResources { get; set; } = new();

    public Guid CurrentIdentityResourceId { get; set; }

    public bool AddIdentityResourceDialogVisible { get; set; }

    public bool UpdateIdentityResourceDialogVisible { get; set; }

    private IdentityResourceService IdentityResourceService => AuthCaller.IdentityResourceService;

    protected override async Task OnInitializedAsync()
    {
        PageName = "IdentityResourceBlock";
        await GetIdentityResourcesAsync();
    }

    public List<DataTableHeader<IdentityResourceDto>> GetHeaders() => new()
    {
        new() { Text = T("Name"), Value = nameof(IdentityResourceDto.Name), Sortable = false, Width="250px"},
        new() { Text = T(nameof(IdentityResourceDto.DisplayName)), Value = nameof(IdentityResourceDto.DisplayName), Sortable = false, Width="250px" },
        new() { Text = T("Required"), Value = nameof(IdentityResourceDto.Required), Sortable = false, Width="105px" },
        new() { Text = T(nameof(IdentityResourceDto.Description)), Value = nameof(IdentityResourceDto.Description), Sortable = false },
        new() { Text = T("State"), Value = nameof(IdentityResourceDto.Enabled), Sortable = false, Width="105px" },
        new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
    };

    public async Task GetIdentityResourcesAsync()
    {
        var reuquest = new GetIdentityResourcesDto(Page, PageSize, Search);
        var response = await IdentityResourceService.GetListAsync(reuquest);
        IdentityResources = response.Items;
        Total = response.Total;
    }

    private async Task AddStandardIdentityResourcesAsync()
    {
        var confirm = await OpenConfirmDialog(T("Operation confirmation"), T("Are you sure create standard identityResources"), AlertTypes.Info);
        if (confirm)
        {
            await IdentityResourceService.AddStandardIdentityResourcesAsync();
            OpenSuccessMessage(T("Add standard identityResource data success"));
            await GetIdentityResourcesAsync();
        }
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
        var confirm = await OpenConfirmDialog(T("Delete IdentityResource"), T("Are you sure delete identityResource \"{0}\"?", identityResource.Name));
        if (confirm) await RemoveIdentityResourceAsync(identityResource.Id);
    }

    public async Task RemoveIdentityResourceAsync(Guid identityResourceId)
    {
        await IdentityResourceService.RemoveAsync(identityResourceId);
        OpenSuccessMessage(T("Delete identityResource data success"));
        await GetIdentityResourcesAsync();
    }
}

