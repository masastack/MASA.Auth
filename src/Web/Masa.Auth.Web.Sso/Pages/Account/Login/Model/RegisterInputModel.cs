// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class RegisterInputModel
{
    public string Password { get; set; } = string.Empty;

    public string ConfirmPassword { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string IdCard { get; set; } = string.Empty;

    public int? EmailCode { get; set; }

    public int? SmsCode { get; set; }

    [MemberNotNullWhen(true, "Email")]
    public bool EmailRegister { get; set; }

    public bool Agreement { get; set; }

    public string DisplayName { get; set; } = string.Empty;

    public string? Account { get; set; }
}
