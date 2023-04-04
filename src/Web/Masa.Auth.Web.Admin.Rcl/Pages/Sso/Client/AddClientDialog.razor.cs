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
    List<string> _tabs = new();
    string _tab = "";

    ClientService ClientService => AuthCaller.ClientService;

    protected override void OnInitialized()
    {
        PageName = "ClientBlock";
        base.OnInitialized();
    }

    public override Task SetParametersAsync(ParameterView parameters)
    {
        if (parameters.TryGetValue(nameof(Culture), out string? culture) && culture?.Equals(Culture) == false)
        {
            _tabs = new List<string> { T("BasicInformation"), T("Resource Information") };
        }
        return base.SetParametersAsync(parameters);
    }

    public void Show()
    {
        _tab = T("BasicInformation");
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
