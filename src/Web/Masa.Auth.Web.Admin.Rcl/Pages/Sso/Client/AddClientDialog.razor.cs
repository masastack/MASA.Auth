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
    ClientAddBasicDto _addBasicDto = new();

    ClientService ClientService => AuthCaller.ClientService;

    private async Task SaveAsync()
    {
        _addBasicDto.Adapt(_addClientDto);

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
