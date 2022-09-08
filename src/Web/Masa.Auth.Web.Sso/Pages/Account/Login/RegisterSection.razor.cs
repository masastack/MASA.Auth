// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class RegisterSection
{
    RegisterInputModel _inputModel = new();
    MForm _registerForm = null!;
    PImageCaptcha _imageCaptcha = null!;
    bool _showPwd, _canSmsCode = true;
    string _codeValue = "";
    System.Timers.Timer? _timer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;

    private Task<string> RefreshCode()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            sb.Append(LoginOptions.RandomCode[Random.Shared.Next(0, LoginOptions.RandomCode.Length)]);
        }

        return Task.FromResult(sb.ToString());
    }

    private async Task RegisterHandler()
    {
        if (await _registerForm.ValidateAsync())
        {
        }
        if (_codeValue == _imageCaptcha.CaptchaCode)
        {

        }
    }

    private async Task GetSmsCode()
    {

    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
