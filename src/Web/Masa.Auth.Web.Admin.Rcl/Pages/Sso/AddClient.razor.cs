namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso;

public partial class AddClient
{
    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    ClientAddDto _clientAddDto = new();
    ClientBasicDto _basicDto = new();
    ClientConsentDto _consentDto = new();
    ClientAuthenticationDto _authenticationDto = new();

    ClientService ClientService => AuthCaller.ClientService;

    private async Task SaveAsync()
    {
        _basicDto.Adapt(_clientAddDto);
        _consentDto.Adapt(_clientAddDto);
        _authenticationDto.Adapt(_clientAddDto);

        //ClientService
    }
}
