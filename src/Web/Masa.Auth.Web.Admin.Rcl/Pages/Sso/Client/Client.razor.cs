namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class Client
{
    bool _addDialog, _updateDialog;
    UpdateClientDialog _updateClientDialog = null!;
    GetClientPaginationDto _clientPaginationDto = new();

    public List<ClientDto> _clients { get; set; } = new();

    public List<DataTableHeader<ClientDto>> _headers { get; set; } = new()
    {
        new() { Text = "名称", Value = nameof(ClientDto.ClientName), Sortable = false },
        new() { Text = "客户端ID", Value = nameof(ClientDto.ClientId), Sortable = false },
        new() { Text = "类型", Value = nameof(ClientDto.ClientType), Sortable = false },
        new() { Text = "描述", Value = nameof(ClientDto.Description), Sortable = false },
        new() { Text = "状态", Value = nameof(ClientDto.Enabled), Sortable = false },
        new() { Text = "操作", Value = "Action", Sortable = false },
    };

    ClientService _clientService => AuthCaller.ClientService;

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
        var data = await _clientService.GetListAsync(_clientPaginationDto);
        _clients = data.Items;
    }

    private async Task OpenUpdateDialog(int clientId)
    {
        await _updateClientDialog.ShowAsync(clientId);
    }
}
