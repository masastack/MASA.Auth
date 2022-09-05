// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public partial class RegisterSection
{
    RegisterInputModel _inputModel = new();
    MForm _registerForm = null!;
    bool _showPwd, _canSmsCode = true;
    string randomCode = "1234567890abcdefghijklmn";
    System.Timers.Timer? _timer;
    int _smsCodeTime = LoginOptions.GetSmsCodeInterval;

    private Task<string> RefreshCode()
    {
        var sb = new StringBuilder();
        for (int i = 0; i < 5; i++)
        {
            sb.Append(randomCode[Random.Shared.Next(0, randomCode.Length)]);
        }

        return Task.FromResult(sb.ToString());
    }

    private async Task GetSmsCode()
    {

    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
