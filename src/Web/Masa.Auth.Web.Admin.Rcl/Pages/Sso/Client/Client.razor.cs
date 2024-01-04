// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class Client
{
    UpdateClientDialog _updateClientDialog = null!;
    GetClientPaginationDto _clientPaginationDto = new();
    PaginationDto<ClientDto> _paginationDto = new();
    AddClientDialog _addClientDialog = null!;

    ClientService _clientService => AuthCaller.ClientService;

    private List<DataTableHeader<ClientDto>> _headers = new();

    protected override void OnInitialized()
    {
        PageName = "ClientBlock";
        
        _headers = new List<DataTableHeader<ClientDto>> {
            new() { Text = T(nameof(ClientDto.ClientName)), Value = nameof(ClientDto.ClientName), Sortable = false , Width="300px"},
            new() { Text = T(nameof(ClientDto.ClientId)), Value = nameof(ClientDto.ClientId), Sortable = false, Width="300px" },
            new() { Text = T(nameof(ClientDto.ClientType)), Value = nameof(ClientDto.ClientType), Sortable = false, Width="105px" },
            new() { Text = T(nameof(ClientDto.Description)), Value = nameof(ClientDto.Description), Sortable = false },
            new() { Text = T(nameof(ClientDto.Enabled)), Value = nameof(ClientDto.Enabled), Sortable = false, Width="105px" },
            new() { Text = T("Action"), Value = "Action", Sortable = false, Align = DataTableHeaderAlign.Center, Width="105px" },
        };
        
        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await LoadData();
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task LoadData()
    {
        _paginationDto = await _clientService.GetListAsync(_clientPaginationDto);
    }

    private async Task OpenUpdateDialog(Guid clientId)
    {
        await _updateClientDialog.ShowAsync(clientId);
    }

    private async Task PageChangedHandler(int page)
    {
        _clientPaginationDto.Page = page;
        await LoadData();
    }

    private async Task PageSizeChangedHandler(int pageSize)
    {
        _clientPaginationDto.PageSize = pageSize;
        await LoadData();
    }

    private async Task SearchKeyHandler()
    {
        _clientPaginationDto.Page = 1;
        await LoadData();
    }

    private async Task OpenRemoveDialog(ClientDto clientDto)
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Client"), T("Are you sure to delete client {0}", clientDto.ClientName));
        if (isConfirmed)
        {
            await _clientService.RemoveClientAsync(clientDto.Id);
            await LoadData();
        }
    }
}
