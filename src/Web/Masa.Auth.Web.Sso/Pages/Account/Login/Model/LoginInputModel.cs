// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public class LoginInputModel
{
    public string UserName { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public string PhoneNumber { get; set; } = string.Empty;

    public int? SmsCode { get; set; }

    public bool RememberLogin { get; set; }

    public bool PhoneLogin { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;

    public string Environment { get; set; } = string.Empty;

    //todo remove this and ldap account auto login
    public bool LdapLogin { get; set; }
}