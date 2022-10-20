// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class LoginSection
{
    [CascadingParameter]
    public string ReturnUrl { get; set; } = string.Empty;

    [Parameter]
    public string LoginHint { get; set; } = string.Empty;

    LoginInputModel _inputModel = new();
    CustomLoginModel? _customLoginModel;
    MForm _loginForm = null!;
    bool _showPwd, _canSmsCode = true, _loginLoading;
    List<EnvironmentModel> _environments = new();
    System.Timers.Timer? _timer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;
    bool _loading;

    protected override void OnInitialized()
    {
        if (_timer == null)
        {
            _timer = new(1000 * 1);
            _timer.Elapsed += Timer_Elapsed;
        }
        base.OnInitialized();
    }

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
                    UserName = LoginHint,
                    Environment = currentEnvironment,
                    RememberLogin = LoginOptions.AllowRememberLogin
                };
            }

            if (ReturnUrl != null && ReturnUrl.Contains('?'))
            {
                var splitIndex = ReturnUrl.IndexOf('?');
                var paramString = ReturnUrl[splitIndex..];
                if (QueryHelpers.ParseQuery(paramString).TryGetValue("client_id", out var clientId))
                {
                    _customLoginModel = await _authClient.CustomLoginService.GetCustomLoginByClientIdAsync(clientId);
                }
            }
            StateHasChanged();
        }
        await base.OnAfterRenderAsync(firstRender);
    }

    private void EnvironmentChanged(string environment)
    {
        _inputModel.Environment = environment;
        ScopedState.Environment = environment;
    }

    private void Timer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _ = InvokeAsync(() =>
        {
            _smsCodeTime--;
            if (_smsCodeTime == 0)
            {
                _timer?.Stop();
                _canSmsCode = true;
                _smsCodeTime = LoginOptions.GetSmsCodeInterval;
            }
            StateHasChanged();
        });
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

    private async Task GetSmsCode()
    {
        var field = _loginForm.EditContext.Field(nameof(_inputModel.PhoneNumber));
        _loginForm.EditContext.NotifyFieldChanged(field);
        var result = _loginForm.EditContext.GetValidationMessages(field);
        if (result.Any() is false)
        {
            _loading = true;
            try
            {
                await _authClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel
                {
                    SendMsgCodeType = SendMsgCodeTypes.Login,
                    PhoneNumber = _inputModel.PhoneNumber
                });
                await PopupService.AlertAsync(T("The verification code is sent successfully, please enter the verification code within 60 seconds"), AlertTypes.Success);
                _canSmsCode = false;
                _timer?.Start();
            }
            catch (Exception e)
            {
                await _js.InvokeVoidAsync("console.log", $"GetSmsCode error: {e.Message}");
                await PopupService.AlertAsync(e.Message, AlertTypes.Error);
            }
            finally
            {
                _loading = false;
            }
        }
    }

    private async Task KeyDownHandler(KeyboardEventArgs args)
    {
        if (args.Key == "Enter")
        {
            await LoginHandler();
        }
    }

    private void NavigateToThirdParty(string scheme)
    {
        var challenge = QueryHelpers.AddQueryString(AuthenticationExternalConstants.ChallengeEndpoint, new Dictionary<string, string?>
        {
            ["returnUrl"] = _inputModel.ReturnUrl,
            ["scheme"] = scheme,
            ["environment"] = _inputModel.Environment
        });
        Navigation.NavigateTo(challenge, true);
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
