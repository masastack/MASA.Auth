namespace Masa.Auth.Web.Admin.Rcl.Pages.Component;

public partial class ClientSelect
{
    [Parameter]
    public string Class { get; set; } = "";

    [Parameter]
    public string Style { get; set; } = "";

    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public int Value { get; set; } = new();

    [Parameter]
    public EventCallback<int> ValueChanged { get; set; }

    public ClientSelectDto? Client { get; set; }

    List<ClientSelectDto> Clients { get; set; } = new();

    ClientService ClientService => AuthCaller.ClientService;

    protected override async Task OnInitializedAsync()
    {
        Clients = await ClientService.GetClientSelectAsync();
    }
}

