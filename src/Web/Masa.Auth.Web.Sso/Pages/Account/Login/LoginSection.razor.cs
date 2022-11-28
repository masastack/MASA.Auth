// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class LoginSection
{
    [CascadingParameter]
    public string ReturnUrl { get; set; } = string.Empty;

    [Parameter]
    public string LoginHint { get; set; } = string.Empty;

    [Parameter]
    public IEnumerable<ViewModel.ExternalProvider> ExternalProviderList { get; set; } = Enumerable.Empty<ViewModel.ExternalProvider>();

    LoginInputModel _inputModel = new();
    MForm _loginForm = null!;
    bool _showPwd, _loginLoading;
    List<EnvironmentModel> _environments = new();

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            _environments = await _pmClient.EnvironmentService.GetListAsync();
            var currentEnvironment = _environments.FirstOrDefault()?.Name ?? "";
            try
            {
                var localEnvironment = await _localStorage.GetAsync<string>(nameof(_inputModel.Environment));
                currentEnvironment = localEnvironment.Value ?? currentEnvironment;
            }
            catch (Exception e)
            {
                await _js.InvokeVoidAsync("console.log", $"ProtectedLocalStorage Get error: {e.Message}");
                await _localStorage.DeleteAsync(nameof(_inputModel.Environment));
            }
            finally
            {
                _inputModel = new LoginInputModel
                {
                    ReturnUrl = ReturnUrl,
                    Account = LoginHint,
                    Environment = currentEnvironment,
                    RememberLogin = LoginOptions.AllowRememberLogin
                };
            }
            (EnvironmentProvider as ISsoEnvironmentProvider)!.SetEnvironment(currentEnvironment);
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void EnvironmentChanged(string environment)
    {
        _inputModel.Environment = environment;
        (EnvironmentProvider as ISsoEnvironmentProvider)!.SetEnvironment(environment);
    }

    private async Task LoginHandler()
    {
        //validate
        if (_loginForm.Validate())
        {
            _loginLoading = true;
            StateHasChanged();
            var msg = await _js.InvokeAsync<string>("login", _inputModel);
            _loginLoading = false;
            if (!string.IsNullOrEmpty(msg))
            {
                await PopupService.AlertAsync(msg, AlertTypes.Error);
            }
            else
            {
                await _localStorage.SetAsync(nameof(_inputModel.Environment), _inputModel.Environment);
                if (SsoUrlHelper.IsLocalUrl(_inputModel.ReturnUrl))
                {
                    Navigation.NavigateTo(_inputModel.ReturnUrl, true);
                }
                else if (string.IsNullOrEmpty(_inputModel.ReturnUrl))
                {
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    await PopupService.AlertAsync("invalid return URL", AlertTypes.Error);
                }
            }
        }
    }

    private async Task Cancel()
    {
        // check if we are in the context of an authorization request
        var context = SsoAuthenticationStateCache.GetAuthorizationContext(_inputModel.ReturnUrl);

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
            Navigation.NavigateTo(_inputModel.ReturnUrl, true);
            return;
        }
        else
        {
            // since we don't have a valid context, then we just go back to the home page
            Navigation.NavigateTo("/", true);
            return;
        }
    }

    private async Task<bool> GetSmsCode()
    {
        var field = _loginForm.EditContext.Field(nameof(_inputModel.PhoneNumber));
        _loginForm.EditContext.NotifyFieldChanged(field);
        var result = _loginForm.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await _authClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel
            {
                SendMsgCodeType = SendMsgCodeTypes.Login,
                PhoneNumber = _inputModel.PhoneNumber
            });
        }
        return !result.Any();
    }

    private async Task KeyDownHandler(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await LoginHandler();
        }
    }
}
