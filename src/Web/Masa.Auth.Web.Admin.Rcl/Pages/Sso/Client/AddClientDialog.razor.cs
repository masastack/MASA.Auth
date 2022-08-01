// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Sso.Client;

public partial class AddClientDialog
{
    [Parameter]
    public bool Visible { get; set; }

    [Parameter]
    public EventCallback<bool> VisibleChanged { get; set; }

    [Parameter]
    public EventCallback OnSuccessed { get; set; }

    AddClientDto _addClientDto = new();
    ClientAddBasicDto _addBasicDto = new();
    ClientScopesDto _clientScopesDto = new();

    ClientService ClientService => AuthCaller.ClientService;

    public async Task Show()
    {
        await Toggle(true);
        _addClientDto = new();
        _addBasicDto = new();
        _clientScopesDto = new();
    }

    private async Task Toggle(bool visible)
    {
        if (VisibleChanged.HasDelegate)
        {
            await VisibleChanged.InvokeAsync(visible);
        }
        else
        {
            Visible = visible;
        }
    }

    private async Task SaveAsync()
    {
        _addBasicDto.Adapt(_addClientDto);
        _clientScopesDto.Adapt(_addClientDto);

        await ClientService.AddClientAsync(_addClientDto);
        await Toggle(false);
        if (OnSuccessed.HasDelegate)
        {
            await OnSuccessed.InvokeAsync();
        }
    }
}
