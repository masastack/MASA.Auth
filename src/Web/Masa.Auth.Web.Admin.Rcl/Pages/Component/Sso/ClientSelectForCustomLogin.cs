// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Sso;

public class ClientSelectForCustomLogin: ClientSelect
{
    protected override async Task OnInitializedAsync()
    {
        Clients = await ClientService.GetClientSelectForCustomLoginAsync();
        Client = Clients.FirstOrDefault(client => client.ClientId == Value);
    }
}
