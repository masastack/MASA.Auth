// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

using IdentityServer4.Validation;

namespace Masa.Auth.Web.Sso.Pages.Consent;

[Authorize]
[SecurityHeaders]
public partial class Index
{
    ViewModel _viewModel = new();
    InputModel _inputModel = new();

    [Parameter]
    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        _viewModel = await BuildViewModelAsync(ReturnUrl);
        _inputModel = new InputModel
        {
            ReturnUrl = ReturnUrl,
        };
        await base.OnInitializedAsync();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && string.IsNullOrWhiteSpace(_viewModel.ClientUrl))
        {
            Navigation.NavigateTo(GlobalVariables.ERROR_ROUTE, true);
            return;
        }
        base.OnAfterRender(firstRender);
    }

    private async Task OnConsent(bool consent)
    {
        // validate return url is still valid
        var request = SsoAuthenticationStateCache.GetAuthorizationContext(ReturnUrl);
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
            await Interaction.GrantConsentAsync(request, grantedConsent);

            // redirect back to authorization endpoint
            if (request.IsNativeClient() == true)
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                Navigation.LoadingPage(_inputModel.ReturnUrl);
                return;
            }
            Navigation.NavigateTo(_inputModel.ReturnUrl, true);
            return;
        }

        // we need to redisplay the consent UI
        _viewModel = await BuildViewModelAsync(_inputModel.ReturnUrl);
    }

    private async Task<ConsentResponse?> AgreeHandler(AuthorizationRequest request)
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

    private async Task<ConsentResponse> RejectHandler(AuthorizationRequest request)
    {
        var grantedConsent = new ConsentResponse { Error = AuthorizationError.AccessDenied };

        // emit event
        await Events.RaiseAsync(new ConsentDeniedEvent(User.GetSubjectId(), request.Client.ClientId, request.ValidatedResources.RawScopeValues));
        return grantedConsent;
    }

    private async Task<ViewModel> BuildViewModelAsync(string returnUrl)
    {
        var request = SsoAuthenticationStateCache.GetAuthorizationContext(returnUrl);
        if (request != null)
        {
            return CreateConsentViewModel(_inputModel, returnUrl, request);
        }
        return await Task.FromResult(new ViewModel());
    }

    private ViewModel CreateConsentViewModel(
        InputModel model, string returnUrl,
        AuthorizationRequest request)
    {
        var vm = new ViewModel
        {
            ClientName = request.Client.ClientName ?? request.Client.ClientId,
            ClientUrl = request.Client.ClientUri,
            ClientLogoUrl = request.Client.LogoUri,
            AllowRememberConsent = request.Client.AllowRememberConsent
        };

        vm.IdentityScopes = request.ValidatedResources.Resources.IdentityResources
            .Select(x => CreateScopeViewModel(x, model?.ScopesConsented == null || model.ScopesConsented?.Contains(x.Name) == true))
            .ToArray();

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
        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
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
            Name = identity.Name,
            Value = identity.Name,
            DisplayName = identity.DisplayName ?? identity.Name,
            Description = identity.Description,
            Emphasize = identity.Emphasize,
            Required = identity.Required,
            Checked = check || identity.Required
        };
    }

    private ScopeViewModel CreateScopeViewModel(ParsedScopeValue parsedScopeValue, ApiScope apiScope, bool check)
    {
        var displayName = apiScope.DisplayName ?? apiScope.Name;
        if (!string.IsNullOrWhiteSpace(parsedScopeValue.ParsedParameter))
        {
            displayName += ":" + parsedScopeValue.ParsedParameter;
        }

        return new ScopeViewModel
        {
            Name = parsedScopeValue.ParsedName,
            Value = parsedScopeValue.RawValue,
            DisplayName = displayName,
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
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }
}
