// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Logout.Model;

public class LogoutViewModel
{
    public string LogoutId { get; set; } = string.Empty;

    public bool ShowLogoutPrompt { get; set; } = true;
}
