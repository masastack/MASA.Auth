// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class UpdateClientDialog
{
    [Parameter]
    public EventCallback OnSuccessed { get; set; }

    [Parameter]
    public EventCallback OnDeleted { get; set; }

    List<string> _tabHeader = new();
    string _tab = "";
    ClientDetailDto _clientDetailDto = new();
    ClientBasicDto _basicDto = new();
    ClientConsentDto _consentDto = new();
    ClientAuthenticationDto _authenticationDto = new();
    ClientScopesDto _clientScopesDto = new();
    ClientDeviceFlowDto _deviceFlowDto = new();
    ClientTokenDto _tokenDto = new();
    ClientCredentialDto _clientCredentialDto = new();
    Type _otherType = null!;
    Dictionary<string, object> _componentMetadata = new();
    bool _visible;

    ClientService ClientService => AuthCaller.ClientService;

    protected override void OnInitialized()
    {
        PageName = "ClientBlock";
        base.OnInitialized();
    }

    public async Task ShowAsync(Guid clientId)
    {
        _clientDetailDto = await ClientService.GetDetailAsync(clientId);
        _clientDetailDto.Adapt(_basicDto);
        _clientDetailDto.Adapt(_consentDto);
        _clientDetailDto.Adapt(_authenticationDto);
        _clientDetailDto.Adapt(_deviceFlowDto);
        _clientDetailDto.Adapt(_tokenDto);
        _clientDetailDto.Adapt(_clientCredentialDto);
        //new object trigger to OnParametersSetAsync
        _clientScopesDto = new();
        _clientDetailDto.Adapt(_clientScopesDto);

        PrepareHeader();
        _tab = T("BasicInformation");
        _visible = true;

        StateHasChanged();
    }

    private void PrepareHeader()
    {
        _tabHeader = new List<string> { T("BasicInformation"), T("Consent Screen"), T("Authentication"), T("Resource Information") };
        if (_clientDetailDto.ClientType == ClientTypes.Device)
        {
            _tabHeader.Add(T("Device Flow"));
            _otherType = typeof(DeviceFlow);
            _componentMetadata = new Dictionary<string, object>{
                { "Dto",_deviceFlowDto }
            };
        }
        else if (_clientDetailDto.ClientType == ClientTypes.Machine)
        {
            _tabHeader.Add(T("Client Secret"));
            _otherType = typeof(ClientSecret);
            _componentMetadata = new Dictionary<string, object>{
                { "Dto",_clientCredentialDto }
            };
        }
        else
        {
            _tabHeader.Add(T("Token"));
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
        _clientScopesDto.Adapt(_clientDetailDto);

        await ClientService.UpdateClientAsync(_clientDetailDto);
        _visible = false;
        if (OnSuccessed.HasDelegate)
        {
            await OnSuccessed.InvokeAsync();
        }
    }

    private async Task DeleteAsync()
    {
        var isConfirmed = await OpenConfirmDialog(T("Delete Client"), T("Are you sure to delete client {0}", _clientDetailDto.ClientName));
        if (isConfirmed)
        {
            await ClientService.RemoveClientAsync(_clientDetailDto.Id);
            _visible = false;
            if (OnDeleted.HasDelegate)
            {
                await OnDeleted.InvokeAsync();
            }
        }
    }
}
