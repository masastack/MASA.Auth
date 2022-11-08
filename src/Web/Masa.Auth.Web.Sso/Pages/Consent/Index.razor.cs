// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Consent;

[Authorize]
[SecurityHeaders]
public partial class Index
{
    ViewModel _viewModel = new();

    [Parameter]
    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; } = string.Empty;

    [Inject]
    public IResourceStore ResourceStore { get; set; } = null!;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var viewModel = BuildViewModelAsync(ReturnUrl);
            if (viewModel is null)
            {
                Navigation.NavigateTo(GlobalVariables.ERROR_ROUTE, true);
                return;
            }
            _viewModel = viewModel;
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void OnConsent(bool consent)
    {
        var queryArguments = new Dictionary<string, string?>()
        {
            { "consent", consent.ToString() },
            { "returnUrl", ReturnUrl },
            { "rememberConsent", _viewModel.RememberConsent.ToString() },
            { "scopes", JsonSerializer.Serialize(_viewModel.ScopesConsented) }
        };

        var url = QueryHelpers.AddQueryString("consent/consent", queryArguments);
        Navigation.NavigateTo(url, true);
    }

    private ViewModel? BuildViewModelAsync(string returnUrl)
    {
        var request = SsoAuthenticationStateCache.GetAuthorizationContext(returnUrl);
        if (request != null)
        {
            return CreateConsentViewModel(returnUrl, request);
        }
        return null;
    }

    private ViewModel CreateConsentViewModel(
        string returnUrl,
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
            .Select(x => CreateScopeViewModel(x, vm.ScopesConsented.Contains(x.Name) == true))
            .ToArray();

        var apiScopes = new List<ScopeViewModel>();
        foreach (var parsedScope in request.ValidatedResources.ParsedScopes)
        {
            var apiScope = request.ValidatedResources.Resources.FindApiScope(parsedScope.ParsedName);
            if (apiScope != null)
            {
                var scopeVm = CreateScopeViewModel(parsedScope, apiScope, vm.ScopesConsented.Contains(parsedScope.RawValue) == true);
                apiScopes.Add(scopeVm);
            }
        }
        if (ConsentOptions.EnableOfflineAccess && request.ValidatedResources.Resources.OfflineAccess)
        {
            apiScopes.Add(GetOfflineAccessScope(vm.ScopesConsented.Contains(IdentityServerConstants.StandardScopes.OfflineAccess) == true));
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
            Name = IdentityServerConstants.StandardScopes.OfflineAccess,
            DisplayName = ConsentOptions.OfflineAccessDisplayName,
            Description = ConsentOptions.OfflineAccessDescription,
            Emphasize = true,
            Checked = check
        };
    }
}
