// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class RegisterUserModel
{
    public string PhoneNumber { get; set; } = string.Empty;

    public int? SmsCode { get; set; }

    public string Email { get; set; } = string.Empty;

    public string EmailCode { get; set; } = string.Empty;

    public string Account { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    public string DisplayName { get; set; } = string.Empty;

    public string Avatar { get; set; } = string.Empty;

    public string ReturnUrl { get; set; } = string.Empty;

    public string Environment { get; set; } = string.Empty;

    public bool EmailRegister { get; set; }

    public bool Agreement { get; set; }
}
