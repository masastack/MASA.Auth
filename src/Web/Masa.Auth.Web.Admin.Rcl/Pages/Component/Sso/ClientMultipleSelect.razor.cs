// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Admin.Rcl.Pages.Component.Sso;

public partial class ClientMultipleSelect
{
    [Parameter]
    public bool Disabled { get; set; }

    [Parameter]
    public List<string> Value { get; set; } = new();

    [Parameter]
    public EventCallback<List<string>> ValueChanged { get; set; }

    [Parameter]
    public Expression<Func<List<string>>>? ValueExpression { get; set; }

    //public ClientSelectDto? Client { get; set; }

    protected List<ClientSelectDto> Clients { get; set; } = new();

    protected ClientService ClientService => AuthCaller.ClientService;

    protected override async Task OnInitializedAsync()
    {
        Clients = await ClientService.GetClientSelectAsync();
        //Client = Clients.FirstOrDefault(client => client.ClientId == Value);
    }

    public async Task UpdateValueAsync(List<string> value)
    {
        //Client = Clients.FirstOrDefault(client => client.ClientId == value);
        if (ValueChanged.HasDelegate) await ValueChanged.InvokeAsync(value);
        else Value = value;
    }
}
