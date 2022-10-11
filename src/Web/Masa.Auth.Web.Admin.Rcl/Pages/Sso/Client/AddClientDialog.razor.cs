// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class AddClientDialog
{
    [Parameter]
    public EventCallback OnSuccessed { get; set; }

    AddClientDto _addClientDto = new();
    AddClientBasicDto _addBasicDto = new();
    ClientScopesDto _clientScopesDto = new();
    bool _visible;
    MForm _form = null!;

    ClientService ClientService => AuthCaller.ClientService;

    public void Show()
    {
        _visible = true;
        _addClientDto = new();
        _addBasicDto = new();
        _clientScopesDto = new();
    }

    private async Task SaveAsync()
    {
        if (!_form.Validate())
        {
            return;
        }
        _addBasicDto.Adapt(_addClientDto);
        _clientScopesDto.Adapt(_addClientDto);

        await ClientService.AddClientAsync(_addClientDto);
        _visible = false;
        if (OnSuccessed.HasDelegate)
        {
            await OnSuccessed.InvokeAsync();
        }
    }
}
