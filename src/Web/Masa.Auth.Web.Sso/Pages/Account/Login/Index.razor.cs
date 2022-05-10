// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

[SecurityHeaders]
[AllowAnonymous]
public partial class Index
{
    ViewModel _viewModel = new();
    InputModel _inputModel = new();
    bool _show;
    MForm? _loginForm;

    [Parameter]
    [SupplyParameterFromQuery]
    public string ReturnUrl { get; set; } = string.Empty;

    [CascadingParameter(Name = "HttpContext")]
    public HttpContext? HttpContext { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await BuildModelAsync(ReturnUrl);
        await base.OnInitializedAsync();
    }
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (_viewModel.IsExternalLoginOnly)
        {
            // we only have one option for logging in and it's an external provider
            Navigation.NavigateTo("ExternalLogin/Challenge");
            return;
            //return RedirectToPage("/ExternalLogin/Challenge", new { scheme = View.ExternalLoginScheme, returnUrl });
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task BuildModelAsync(string returnUrl)
    {
        _inputModel = new InputModel
        {
            ReturnUrl = returnUrl
        };

        var context = await Interaction.GetAuthorizationContextAsync(returnUrl);
        if (context?.IdP != null && await _schemeProvider.GetSchemeAsync(context.IdP) != null)
        {
            var local = context.IdP == IdentityServerConstants.LocalIdentityProvider;

            // this is meant to short circuit the UI and only trigger the one external IdP
            _viewModel = new ViewModel
            {
                EnableLocalLogin = local,
            };

            _inputModel.Username = context?.LoginHint ?? "";

            if (!local)
            {
                _viewModel.ExternalProviders = new[] { new ViewModel.ExternalProvider { AuthenticationScheme = context?.IdP ?? "" } };
            }

            return;
        }

        var schemes = await _schemeProvider.GetAllSchemesAsync();

        var providers = schemes
            .Where(x => x.DisplayName != null)
            .Select(x => new ViewModel.ExternalProvider
            {
                DisplayName = x.DisplayName ?? x.Name,
                AuthenticationScheme = x.Name
            }).ToList();

        var dyanmicSchemes = (await _identityProviderStore.GetAllSchemeNamesAsync())
            .Where(x => x.Enabled)
            .Select(x => new ViewModel.ExternalProvider
            {
                AuthenticationScheme = x.Scheme,
                DisplayName = x.DisplayName
            });
        providers.AddRange(dyanmicSchemes);

        var allowLocal = true;
        if (context?.Client.ClientId != null)
        {
            var client = await _clientStore.FindEnabledClientByIdAsync(context.Client.ClientId);
            if (client != null)
            {
                allowLocal = client.EnableLocalLogin;
                if (client.IdentityProviderRestrictions != null && client.IdentityProviderRestrictions.Any())
                {
                    providers = providers.Where(provider => client.IdentityProviderRestrictions.Contains(provider.AuthenticationScheme)).ToList();
                }
            }
        }

        _viewModel = new ViewModel
        {
            AllowRememberLogin = LoginOptions.AllowRememberLogin,
            EnableLocalLogin = allowLocal && LoginOptions.AllowLocalLogin,
            ExternalProviders = providers.ToArray()
        };
    }

    private async Task Login()
    {
        // check if we are in the context of an authorization request
        var context = await Interaction.GetAuthorizationContextAsync(_inputModel.ReturnUrl);

        //validate
        if (_loginForm != null && await _loginForm.ValidateAsync())
        {
            if (HttpContext != null)
            {
                await HttpContext.SignOutAsync();
            }

            Navigation.NavigateTo($"login?userName={_inputModel.Username}&password={_inputModel.Password}&remember_login={_inputModel.RememberLogin}&returnUrl={_inputModel.ReturnUrl}", true);
        }
        // something went wrong, show form with error
        await BuildModelAsync(_inputModel.ReturnUrl);
    }

    private async Task Cancel()
    {
        // check if we are in the context of an authorization request
        var context = await Interaction.GetAuthorizationContextAsync(_inputModel.ReturnUrl);

        if (context != null)
        {
            // if the user cancels, send a result back into IdentityServer as if they 
            // denied the consent (even if this client does not require consent).
            // this will send back an access denied OIDC error response to the client.
            await Interaction.DenyAuthorizationAsync(context, AuthorizationError.AccessDenied);

            // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
            if (context.IsNativeClient())
            {
                // The client is native, so this change in how to
                // return the response is for better UX for the end user.
                Navigation.LoadingPage(_inputModel.ReturnUrl);
                return;
            }
            Navigation.NavigateTo(_inputModel.ReturnUrl);
            return;
        }
        else
        {
            // since we don't have a valid context, then we just go back to the home page
            Navigation.NavigateTo("/");
            return;
        }
    }
}
