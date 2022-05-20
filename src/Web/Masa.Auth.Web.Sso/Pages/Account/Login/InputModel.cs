// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Login;

public class InputModel
{
    [Required]
    public string Username { get; set; } = "andy@hotmail.com";

    [Required]
    public string Password { get; set; } = "andypassword";

    public bool RememberLogin { get; set; }

    public string ReturnUrl { get; set; } = string.Empty;
}