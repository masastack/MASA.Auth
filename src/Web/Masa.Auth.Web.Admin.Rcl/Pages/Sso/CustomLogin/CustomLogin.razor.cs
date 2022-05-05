// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.CustomLogin;

public partial class CustomLogin
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
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _pageSize = value;
            GetCustomLoginsAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<CustomLoginDto> CustomLogins { get; set; } = new();

    public int CurrentCustomLoginId { get; set; }

    public bool AddCustomLoginDialogVisible { get; set; }

    public bool UpdateCustomLoginDialogVisible { get; set; }

    public List<DataTableHeader<CustomLoginDto>> Headers { get; set; } = new();

    private CustomLoginService CustomLoginService => AuthCaller.CustomLoginService;

    protected override async Task OnInitializedAsync()
    {
        Headers = new()
        {
            new() { Text = T(nameof(ClientDto.LogoUri)), Value = nameof(ClientDto.LogoUri), Sortable = false },
            new() { Text = T("CustomLogin.Name"), Value = nameof(CustomLoginDto.Name), Sortable = false },
            new() { Text = T("ClientDto.ClientName"), Value = nameof(ClientDto.ClientName), Sortable = false },
            new() { Text = T(nameof(CustomLoginDto.CreationTime)), Value = nameof(CustomLoginDto.CreationTime), Sortable = false },
            new() { Text = T(nameof(CustomLoginDto.ModificationTime)), Value = nameof(CustomLoginDto.ModificationTime), Sortable = false },
            new() { Text = T(nameof(CustomLoginDto.Modifier)), Value = nameof(CustomLoginDto.Modifier), Sortable = false },
            new() { Text = T("State"), Value = nameof(CustomLoginDto.Enabled), Sortable = false },
            new() { Text = T("Action"), Value = "Action", Sortable = false },
        };

        await GetCustomLoginsAsync();
    }

    public async Task GetCustomLoginsAsync()
    {
        Loading = true;
        var reuquest = new GetCustomLoginsDto(Page, PageSize, Search);
        var response = await CustomLoginService.GetListAsync(reuquest);
        CustomLogins = response.Items;
        Total = response.Total;
        Loading = false;
    }

    public void OpenAddCustomLoginDialog()
    {
        AddCustomLoginDialogVisible = true;
    }

    public void OpenUpdateCustomLoginDialog(CustomLoginDto CustomLogin)
    {
        CurrentCustomLoginId = CustomLogin.Id;
        UpdateCustomLoginDialogVisible = true;
    }

    public async Task OpenRemoveCustomLoginDialog(CustomLoginDto CustomLogin)
    {
        var confirm = await OpenConfirmDialog(T("Are you sure delete customLogin data"));
        if (confirm) await RemoveCustomLoginAsync(CustomLogin.Id);
    }

    public async Task RemoveCustomLoginAsync(int CustomLoginId)
    {
        Loading = true;
        await CustomLoginService.RemoveAsync(CustomLoginId);
        OpenSuccessMessage(T("Delete customLogin data success"));
        await GetCustomLoginsAsync();
        Loading = false;
    }
}

