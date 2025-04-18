﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.RolePermissions.Roles;

public partial class Role
{
    private string? _search;
    private bool? _enabled;
    private string _clientId = string.Empty;
    private int _page = 1, _pageSize = 20;

    public string Search
    {
        get { return _search ?? ""; }
        set
        {
            _search = value;
            _page = 1;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public bool? Enabled
    {
        get { return _enabled; }
        set
        {
            _enabled = value;
            _page = 1;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public string ClientId
    {
        get { return _clientId; }
        set
        {
            _clientId = value;
            _page = 1;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int Page
    {
        get { return _page; }
        set
        {
            _page = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public int PageSize
    {
        get { return _pageSize; }
        set
        {
            _page = 1;
            _pageSize = value;
            GetRolesAsync().ContinueWith(_ => InvokeAsync(StateHasChanged));
        }
    }

    public long Total { get; set; }

    public List<RoleDto> Roles { get; set; } = new();

    public Guid CurrentRoleId { get; set; }

    public bool AddRoleDialogVisible { get; set; }

    public bool UpdateRoleDialogVisible { get; set; }

    public bool RoleUsersDialogVisible { get; set; }

    private RoleService RoleService => AuthCaller.RoleService;

    protected override string? PageName => "RoleBlock";

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await GetRolesAsync();
        }
        await base.OnAfterRenderAsync(firstRender);
    }
    
    private List<DataTableHeader<RoleDto>> _headers = new();

    protected override void OnInitialized()
    {
        _headers = new List<DataTableHeader<RoleDto>>() {
            new() { Text = T("Name"), Value = nameof(RoleDto.Name), Sortable = false, Align = DataTableHeaderAlign.Start },
            new() {
                Text = T(nameof(RoleDto.CreationTime)), Value = nameof(RoleDto.CreationTime), Sortable = false, Align = DataTableHeaderAlign.Start
            },
            new() {
                Text = T(nameof(RoleDto.ModificationTime)), Value = nameof(RoleDto.ModificationTime), Sortable = false,
                Align = DataTableHeaderAlign.Start
            },
            new() { Text = T(nameof(RoleDto.Creator)), Value = nameof(RoleDto.Creator), Sortable = false, Align = DataTableHeaderAlign.Start },
            new() { Text = T(nameof(RoleDto.Modifier)), Value = nameof(RoleDto.Modifier), Sortable = false, Align = DataTableHeaderAlign.Start },
            new() { Text = T("State"), Value = nameof(RoleDto.Enabled), Sortable = false, Align = DataTableHeaderAlign.Start, Width = "105px" },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width = "105px" },
        };
        
        base.OnInitialized();
    }

    public async Task GetRolesAsync()
    {
        PopupService.ShowProgressLinear();
        var request = new GetRolesDto(Page, PageSize, Search, Enabled, ClientId);
        var response = await RoleService.GetListAsync(request);
        PopupService.HideProgressLinear();
        Roles = response.Items;
        Total = response.Total;
        StateHasChanged();
    }

    public void OpenAddRoleDialog()
    {
        AddRoleDialogVisible = true;
    }

    public void OpenUpdateRoleDialog(RoleDto role)
    {
        CurrentRoleId = role.Id;
        UpdateRoleDialogVisible = true;
    }

    public void OpenRoleUsersDialog(RoleDto role)
    {
        CurrentRoleId = role.Id;
        RoleUsersDialogVisible = true;
    }

    public async Task OpenRemoveRoleDialog(RoleDto role)
    {
        var confirm = await OpenConfirmDialog(T("Delete Role"), T("Are you sure delete role \"{0}\"?", role.Name));
        if (confirm) await RemoveRoleAsync(role.Id);
    }

    public async Task RemoveRoleAsync(Guid roleId)
    {
        await RoleService.RemoveAsync(roleId);
        OpenSuccessMessage(T("Delete role data success"));
        await GetRolesAsync();
    }
}

