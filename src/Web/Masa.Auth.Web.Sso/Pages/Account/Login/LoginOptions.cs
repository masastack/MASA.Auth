// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public class LoginOptions
{
    public static bool AllowLocalLogin = true;
    public static bool AllowRememberLogin = true;
    public static TimeSpan RememberMeLoginDuration = TimeSpan.FromDays(30);
    public static string InvalidCredentialsErrorMessage = "Invalid username or password";
    public static readonly string PhoneRegular = @"^\s{0}$|^((\+86)|(86))?(1[3-9][0-9])\d{8}$";
    public static readonly int GetSmsCodeInterval = 60;
    public static readonly TimeSpan SmsCodeExpire = TimeSpan.FromMinutes(5);
}