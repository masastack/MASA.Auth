using Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client.Section;

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class UpdateClientDialog
{
    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    [Parameter]
    public EventCallback OnSuccessed { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    List<string> _tabHeader = new();
    ClientDetailDto _clientDetailDto = new();
    ClientBasicDto _basicDto = new();
    ClientConsentDto _consentDto = new();
    ClientAuthenticationDto _authenticationDto = new();
    ClientDeviceFlowDto _deviceFlowDto = new();
    ClientTokenDto _tokenDto = new();
    ClientCredentialDto _clientCredentialDto = new();
    Type _otherType = null!;
    Dictionary<string, object> _componentMetadata = new();

    ClientService ClientService => AuthCaller.ClientService;

    public async Task ShowAsync(int clientId)
    {
        _clientDetailDto = await ClientService.GetDetailAsync(clientId);
        _clientDetailDto.Adapt(_basicDto);
        _clientDetailDto.Adapt(_consentDto);
        _clientDetailDto.Adapt(_authenticationDto);
        _clientDetailDto.Adapt(_deviceFlowDto);
        _clientDetailDto.Adapt(_tokenDto);
        _clientDetailDto.Adapt(_clientCredentialDto);

        PrepareHeader();

        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(true);
        }
        else
        {
            Value = true;
        }
        StateHasChanged();
    }

    private void PrepareHeader()
    {
        if (_clientDetailDto.ClientType == ClientTypes.Device)
        {
            _tabHeader = new List<string> { "Basic", "Consent Screen", "Authentication", "Resource", "Device Flow" };
            _otherType = typeof(DeviceFlow);
            _componentMetadata = new Dictionary<string, object>{
                { "Dto",_deviceFlowDto }
            };
        }
        else if (_clientDetailDto.ClientType == ClientTypes.Machine)
        {
            _tabHeader = new List<string> { "Basic", "Consent Screen", "Authentication", "Resource", "Client Secret" };
            _otherType = typeof(ClientSecret);
            _componentMetadata = new Dictionary<string, object>{
                { "Dto",_clientCredentialDto }
            };
        }
        else
        {
            _tabHeader = new List<string> { "Basic", "Consent Screen", "Authentication", "Resource", "Token" };
            _otherType = typeof(Token);
            _componentMetadata = new Dictionary<string, object>{
                { "Dto",_tokenDto }
            };
        }
    }

    private async Task SaveAsync()
    {
        _basicDto.Adapt(_clientDetailDto);
        _consentDto.Adapt(_clientDetailDto);
        _authenticationDto.Adapt(_clientDetailDto);
        _deviceFlowDto.Adapt(_clientDetailDto);
        _tokenDto.Adapt(_clientDetailDto);
        _clientCredentialDto.Adapt(_clientDetailDto);

        await ClientService.UpdateClientAsync(_clientDetailDto);
        await CloseAsync();
        if (OnSuccessed.HasDelegate)
        {
            await OnSuccessed.InvokeAsync();
        }
    }

    private async Task DeleteAsync()
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Client"), T("Are you sure you want to delete this client"), AlertTypes.Warning);
        if (isConfirmed)
        {
            await ClientService.RemoveClientAsync(_clientDetailDto.Id);
            await CloseAsync();
            if (OnDeleted.HasDelegate)
            {
                await OnDeleted.InvokeAsync();
            }
        }
    }

    private async Task CloseAsync()
    {
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(false);
        }
        else
        {
            Value = false;
        }
    }
}
