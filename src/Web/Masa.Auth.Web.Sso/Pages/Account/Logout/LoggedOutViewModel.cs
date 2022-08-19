// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Web.Sso.Pages.Account.Logout;

public class LoggedOutViewModel
{
    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string SignOutIframeUrl { get; set; } = string.Empty;

    public bool AutomaticRedirectAfterSignOut { get; set; }
}