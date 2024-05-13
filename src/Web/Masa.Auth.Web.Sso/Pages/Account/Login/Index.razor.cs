// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

[AllowAnonymous]
public partial class Index
{
    ViewModel _viewModel = new();
    private string _loginHint = "";
    string _clientId = "";
    private static readonly string[] AcceptTabs = { "login", "register" };

    [CascadingParameter(Name = "Culture")] private string? Culture { get; set; }

    [Parameter] public string? Tab { get; set; }

    [Parameter] [SupplyParameterFromQuery] public string ReturnUrl { get; set; } = string.Empty;

    private string? ComputedTab => Tab?.ToString();

    protected override void OnInitialized()
    {
        _environmentData.EnvironmentChanged += EnvironmentChanged;
    }

    protected async override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await BuildModelAsync(ReturnUrl);
            if (_viewModel.IsExternalLoginOnly)
            {
                // we only have one option for logging in and it's an external provider
                Navigation.NavigateTo(AuthenticationExternalConstants.ChallengeEndpoint, new Dictionary<string, object?> {
                    { "scheme", _viewModel.ExternalLoginScheme },
                    { "returnUrl", ReturnUrl }
                });
                return;
            }

            StateHasChanged();
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        var context = SsoAuthenticationStateCache.GetAuthorizationContext(returnUrl);
        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            _viewModel = new ViewModel {
                EnableLocalLogin = local,
            };

            _loginHint = context?.LoginHint ?? "";

            if (!local)
            {
                _viewModel.ExternalProviders = new[]
                    { new ViewModel.ExternalProvider { AuthenticationScheme = context?.IdP ?? "" } };
            }

            return;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var allowLocal = true;
        if (context?.Client.ClientId != null)
        {
            _clientId = context.Client.ClientId;
            var client = await _clientStore.FindEnabledClientByIdAsync(_clientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;
                //todo IdentityProviderRestrictions linkage auth ThirdPartyIdps
                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    providers = providers.Where(provider =>
                        client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        _viewModel = new ViewModel {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }

    private void TabChanged(StringNumber? tab)
    {
        Tab = tab?.ToString();

        if (AcceptTabs.Contains(Tab))
        {
            Navigation.Replace($"/account/{Tab}");
        }
    }

    async void EnvironmentChanged(object? sender, EnvironmentDataEventArgs e)
    {
        var customLoginModel = await _authClient.CustomLoginService.GetCustomLoginByClientIdAsync(e.Value, _clientId);
        if (customLoginModel != null)
        {
            _viewModel.ExternalProviders = customLoginModel.ThirdPartyIdps.Select(idp => new ViewModel.ExternalProvider
            {
                DisplayName = idp.DisplayName ?? idp.Name,
                AuthenticationScheme = idp.Name,
                Icon = idp.Icon
            }).ToList();
        
            _viewModel.RegisterFields = customLoginModel.RegisterFields;
            await InvokeAsync(StateHasChanged);
        }
    }

    public void Dispose()
    {
        _environmentData.EnvironmentChanged -= EnvironmentChanged;
    }
}