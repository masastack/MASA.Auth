// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class AddClientDialog
{
    [Parameter]
    public bool Value { get; set; }

    [Parameter]
    public EventCallback<bool> ValueChanged { get; set; }

    [Parameter]
    public EventCallback OnSuccessed { get; set; }

    AddClientDto _addClientDto = new();
    ClientBasicDto _basicDto = new();
    ClientConsentDto _consentDto = new();
    ClientAuthenticationDto _authenticationDto = new();

    ClientService ClientService => AuthCaller.ClientService;

    private async Task SaveAsync()
    {
        _basicDto.Adapt(_addClientDto);
        _consentDto.Adapt(_addClientDto);
        _authenticationDto.Adapt(_addClientDto);

        await ClientService.AddClientAsync(_addClientDto);
        if (ValueChanged.HasDelegate)
        {
            await ValueChanged.InvokeAsync(false);
        }
        else
        {
            Value = false;
        }
        if (OnSuccessed.HasDelegate)
        {
            await OnSuccessed.InvokeAsync();
        }
    }
}
