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
    public static readonly string EmailRegular = @"^\s{0}$|^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
    public static readonly string PasswordRegular = @"^\s{0}$|^\S*(?=\S{6,})(?=\S*\d)(?=\S*[A-Za-z])\S*$";
    public static readonly string IdCard = "^\\s{0}$|(^\\d{15}$)|(^\\d{17}([0-9]|X|x)$)";
    public static readonly int GetEmailCodeInterval = 60;
    public static readonly int CodeMaximum = 999999;
    public static readonly int CodeMinimum = 100000;
}