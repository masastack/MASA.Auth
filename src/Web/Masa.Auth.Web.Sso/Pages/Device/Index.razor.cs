// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityServer4.Validation;
using Masa.Auth.Web.Sso.Pages.Consent;

namespace Masa.Auth.Web.Sso.Pages.Device;

public partial class Index
{
    ViewModel _viewModel = new();
    InputModel _inputModel = new();

    [Parameter]
    [SupplyParameterFromQuery]
    public string UserCode { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _viewModel = await BuildViewModelAsync(UserCode);
        _inputModel = new InputModel
        {
            UserCode = UserCode,
        };
        await base.OnInitializedAsync();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task OnDevice(bool consent)
    {
        // validate return url is still valid
        var request = await _interaction.GetAuthorizationContextAsync(UserCode);
        if (request == null)
        {
            Navigation.NavigateTo(GlobalVariables.ERROR_ROUTE, true);
            return;
        }

        ConsentResponse? grantedConsent = null;
        if (consent)
        {
            grantedConsent = await AgreeHandler(request);
        }
        else
        {
            grantedConsent = await RejectHandler(request);
        }

        if (grantedConsent != null)
        {
            // communicate outcome of consent back to identityserver
            await _interaction.HandleRequestAsync(UserCode, grantedConsent);

            Navigation.NavigateTo("device/success", true);
            return;
        }

        // we need to redisplay the consent UI
        _viewModel = await BuildViewModelAsync(_inputModel.ReturnUrl);
    }

    private async Task<ConsentResponse?> AgreeHandler(DeviceFlowAuthorizationRequest request)
    {
        ConsentResponse? grantedConsent = null;
        // if the user consented to some scope, build the response model
        if (_inputModel.ScopesConsented != null && _inputModel.ScopesConsented.Any())
        {
            var scopes = _inputModel.ScopesConsented;
            if (ConsentOptions.EnableOfflineAccess == false)
            {
                scopes = scopes.Where(x => x != IdentityServerConstants.StandardScopes.OfflineAccess);
            }

            grantedConsent = new ConsentResponse
            {
                RememberConsent = _inputModel.RememberConsent,
                ScopesValuesConsented = scopes.ToArray(),
                Description = _inputModel.Description
            };

            // emit event
            await Events.RaiseAsync(new ConsentGrantedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues, grantedConsent.ScopesValuesConsented, grantedConsent.RememberConsent));
        }
        else
        {
            await PopupService.AlertAsync(ConsentOptions.MustChooseOneErrorMessage, BlazorComponent.AlertTypes.Error);
        }
        return grantedConsent;
    }

    private async Task<ConsentResponse> RejectHandler(DeviceFlowAuthorizationRequest request)
    {
        var grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

        // emit event
        await Events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
        return grantedConsent;
    }

    private async Task<ViewModel> BuildViewModelAsync(string userCode)
    {
        var request = await _interaction.GetAuthorizationContextAsync(userCode);
        if (request != null)
        {
            return CreateConsentViewModel(_inputModel, request);
        }
        return await Task.FromResult(new ViewModel());
    }

    private ViewModel CreateConsentViewModel(InputModel model, DeviceFlowAuthorizationRequest request)
    {
        var client = request.Client;
        var vm = new ViewModel
        {
            ClientName = client.ClientName ?? client.ClientId,
            ClientUrl = client.ClientUri,
            ClientLogoUrl = client.LogoUri,
            AllowRememberConsent = client.AllowRememberConsent
        };

        vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources.Select(x => CreateScopeViewModel(x, model == null || model.ScopesConsented?.Contains(x.Name) == true)).ToArray();

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, model == null || model.ScopesConsented?.Contains(parsedScope.RawValue) == true);
                apiScopes.Add(scopeVm);
            }
        }
        if (DeviceOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(model == null || model.ScopesConsented?.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) == true));
        }
        vm.ApiScopes = apiScopes;

        return vm;
    }

    private ScopeViewModel CreateScopeViewModel(IdentityResource identity, bool check)
    {
        return new ScopeViewModel
        {
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    public ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        return new ScopeViewModel
        {
            Value = parsedScopeValue.RawValue,
            DisplayName = apiScope.DisplayName ?? apiScope.Name,
            Description = apiScope.Description,
            Emphasize = apiScope.Emphasize,
            Required = apiScope.Required,
            Checked = check || apiScope.Required
        };
    }

    private ScopeViewModel GetOfflineAccessScope(bool check)
    {
        return new ScopeViewModel
        {
            Value = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = DeviceOptions.OfflineAccessDisplayName,
            Description = DeviceOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }
}
