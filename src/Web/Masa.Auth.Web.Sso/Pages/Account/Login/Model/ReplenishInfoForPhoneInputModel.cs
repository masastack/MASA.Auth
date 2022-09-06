// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login.Model;

public class ReplenishInfoForPhoneInputModel : ReplenishInfoInputModel
{
    public string Email { get; set; } = string.Empty;

    public string Passwrod { get; set; } = string.Empty;

    public string ConfirmPasswrod { get; set; } = string.Empty;
}
