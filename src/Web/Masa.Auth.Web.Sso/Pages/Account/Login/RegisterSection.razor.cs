// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class RegisterSection
{
    [Inject]
    public IAuthClient AuthClient { get; set; } = null!;

    RegisterInputModel _inputModel = new();
    MForm _registerForm = null!;
    PImageCaptcha _imageCaptcha = null!;
    bool _showPwd, _canSmsCode = true;
    string _codeValue = "";
    System.Timers.Timer? _timer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;

    private Task<string> RefreshCode()
    {
        var arrCode = new char[5];
        for (var i = 0; i < 5; i++)
        {
            arrCode[i] = LoginOptions.RandomCode[Random.Shared.Next(0, LoginOptions.RandomCode.Length)];
        }
        return Task.FromResult(new string(arrCode));
    }

    private async Task RegisterHandler()
    {
        if (!_registerForm.Validate())
        {
            return;
        }
        if (_inputModel.EmailRegister)
        {
            if (_codeValue.ToLower() != _imageCaptcha.CaptchaCode.ToLower())
            {
                await PopupService.AlertAsync(T("EmailCodePrompt"), AlertTypes.Error);
                return;
            }
        }
        else
        {
            //phone code verify
            if (true)
            {
                await PopupService.AlertAsync(T("PhoneCodePrompt"), AlertTypes.Error);
                return;
            }
        }
        //todo change register api
        var userModel = await AuthClient.UserService.AddAsync(new AddUserModel
        {
            Email = _inputModel.Email,
            Password = _inputModel.Password,
            PhoneNumber = _inputModel.PhoneNumber,
            Account = _inputModel.EmailRegister ? _inputModel.Email : _inputModel.PhoneNumber,
            Avatar = "",
            DisplayName = GenerateDisplayName(_inputModel)
        });

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
            _timer?.Start();
        }
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
