// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class RegisterSuccess
{
    [Inject]
    public IAuthClient AuthClient { get; set; } = null!;

    bool _canSmsCode = true, _showPwd, _registerLoading;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;
    System.Timers.Timer? _smsTimer;
    MForm _registerForm = null!;

    protected override void OnInitialized()
    {
        if (_smsTimer == null)
        {
            _smsTimer = new(1000 * 1);
            _smsTimer.Elapsed += SmsTimer_Elapsed;
        }
        base.OnInitialized();
    }

    private void SmsTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _ = InvokeAsync(() =>
        {
            _smsCodeTime--;
            if (_smsCodeTime == 0)
            {
                _smsTimer?.Stop();
                _canSmsCode = true;
                _smsCodeTime = LoginOptions.GetSmsCodeInterval;
            }
            StateHasChanged();
        });
    }

    private async Task GetSmsCode()
    {
        var field = _registerForm.EditContext.Field(nameof(RegisterUser.PhoneNumber));
        _registerForm.EditContext.NotifyFieldChanged(field);
        var result = _registerForm.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel
            {
                SendMsgCodeType = SendMsgCodeTypes.Register,
                PhoneNumber = RegisterUser.PhoneNumber
            });
            await PopupService.AlertAsync(T("The verification code is sent successfully, please enter the verification code within 60 seconds"), AlertTypes.Success);
            _canSmsCode = false;
            _smsTimer?.Start();
        }
    }

    private async Task RegisterUserAsync()
    {
        if (_registerForm.Validate())
        {
            var loginInputModel = new LoginInputModel
            {
                PhoneLogin = !RegisterUser.EmailRegister,
                SmsCode = RegisterUser.SmsCode,
                Password = RegisterUser.Password,
                UserName = RegisterUser.Email,
                Environment = RegisterUser.Environment,
                PhoneNumber = RegisterUser.PhoneNumber,
                ReturnUrl = RegisterUser.ReturnUrl,
                RegisterLogin = true
            };
            var msg = await _js.InvokeAsync<string>("login", loginInputModel);
            _registerLoading = false;
            if (!string.IsNullOrEmpty(msg))
            {
                await PopupService.AlertAsync(msg, AlertTypes.Error);
            }
            else
            {
                if (SsoUrlHelper.IsLocalUrl(RegisterUser.ReturnUrl))
                {
                    Navigation.NavigateTo(RegisterUser.ReturnUrl, true);
                }
                else if (string.IsNullOrEmpty(RegisterUser.ReturnUrl))
                {
                    Navigation.NavigateTo("/", true);
                }
                else
                {
                    await PopupService.AlertAsync("invalid return url", AlertTypes.Error);
                }
            }
        }
    }

    public void Dispose()
    {
        _smsTimer?.Dispose();
    }
}
