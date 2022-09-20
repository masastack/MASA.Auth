// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class RegisterSection
{
    [Inject]
    public IAuthClient AuthClient { get; set; } = null!;

    [CascadingParameter]
    public string ReturnUrl { get; set; } = string.Empty;

    RegisterInputModel _inputModel = new();
    MForm _registerForm = null!;
    bool _showPwd, _canSmsCode = true, _canEmailCode = true;
    System.Timers.Timer? _smsTimer, _emailTimer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval, _emailCodeTime = LoginOptions.GetEmailCodeInterval;

    public bool CanRegister => _inputModel.Agreement &&
            (_inputModel.EmailRegister ? _inputModel.EmailCode.HasValue : _inputModel.SmsCode.HasValue);

    protected override void OnInitialized()
    {
        if (_smsTimer == null)
        {
            _smsTimer = new(1000 * 1);
            _smsTimer.Elapsed += SmsTimer_Elapsed;
        }
        if (_emailTimer == null)
        {
            _emailTimer = new(1000 * 1);
            _emailTimer.Elapsed += EmailTimer_Elapsed;
        }
        base.OnInitialized();
    }

    protected override void OnAfterRender(bool firstRender)
    {
        if (firstRender && ReturnUrl?.Contains('?') == true)
        {
            var splitIndex = ReturnUrl.IndexOf('?');
            var baseString = ReturnUrl[..splitIndex];
            var paramString = ReturnUrl[splitIndex..];
            var queryValues = HttpUtility.ParseQueryString(paramString);
            queryValues["redirect_uri"] = $"{queryValues["redirect_uri"]}/user-center";
            var d = $"{baseString}?{queryValues}";
        }
        base.OnAfterRender(firstRender);
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

    private void EmailTimer_Elapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        _ = InvokeAsync(() =>
        {
            _emailCodeTime--;
            if (_emailCodeTime == 0)
            {
                _emailTimer?.Stop();
                _canEmailCode = true;
                _emailCodeTime = LoginOptions.GetEmailCodeInterval;
            }
            StateHasChanged();
        });
    }

    private async Task RegisterHandler()
    {
        if (!_registerForm.Validate())
        {
            return;
        }
        RegisterUser.Account = _inputModel.EmailRegister ? _inputModel.Email : _inputModel.PhoneNumber;
        RegisterUser.DisplayName = GenerateDisplayName(_inputModel);
        RegisterUser.Email = _inputModel.Email;
        RegisterUser.PhoneNumber = _inputModel.PhoneNumber;
        RegisterUser.ReturnUrl = ReturnUrl;
        RegisterUser.EmailRegister = _inputModel.EmailRegister;

        if (!_inputModel.EmailRegister)
        {
            //await AuthClient.UserService.RegisterByPhoneAsync(new RegisterByPhoneModel
            //{
            //    PhoneNumber = _inputModel.PhoneNumber,
            //    SmsCode = _inputModel.SmsCode.ToString() ?? "",
            //    Account = _inputModel.PhoneNumber,
            //    Avatar = "",
            //    DisplayName = RegisterUserState.RegisterUser.Account
            //});
        }

        Navigation.NavigateTo("/RegisteredSuccess");

        string GenerateDisplayName(RegisterInputModel _inputModel)
        {
            var _prefix = T("User");
            var _suffix = "";
            if (_inputModel.EmailRegister)
            {
                _suffix = _inputModel.Email[.._inputModel.Email.IndexOf('@')];
            }
            else
            {
                _suffix = _inputModel.PhoneNumber[7..];
            }
            return _prefix + _suffix;
        }
    }

    private async Task GetSmsCode()
    {
        var field = _registerForm.EditContext.Field(nameof(_inputModel.PhoneNumber));
        _registerForm.EditContext.NotifyFieldChanged(field);
        var result = _registerForm.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendMsgCodeAsync(new SendMsgCodeModel
            {
                SendMsgCodeType = SendMsgCodeTypes.Register,
                PhoneNumber = _inputModel.PhoneNumber
            });
            await PopupService.AlertAsync(T("The verification code is sent successfully, please enter the verification code within 60 seconds"), AlertTypes.Success);
            _canSmsCode = false;
            _smsTimer?.Start();
        }
    }

    private async Task GetEmailCode()
    {
        var field = _registerForm.EditContext.Field(nameof(_inputModel.Email));
        _registerForm.EditContext.NotifyFieldChanged(field);
        var result = _registerForm.EditContext.GetValidationMessages(field);
        if (!result.Any())
        {
            await AuthClient.UserService.SendEmailAsync(new SendEmailModel
            {
                Email = _inputModel.Email,
                SendEmailType = SendEmailTypes.Register
            });
            await PopupService.AlertAsync(T("The verification code is sent successfully, please enter the verification code within 60 seconds"), AlertTypes.Success);
            _canEmailCode = false;
            _emailTimer?.Start();
        }
    }

    public void Dispose()
    {
        _smsTimer?.Dispose();
        _emailTimer?.Dispose();
    }
}
