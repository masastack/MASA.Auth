// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddClientBasicDto
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> GrantTypes { get; set; } = new();

    public string ClientUri { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public bool RequireConsent { get; set; } = true;

    public bool AllowOfflineAccess { get; set; }

    public string RedirectUri { get; set; } = string.Empty;

    public List<string> RedirectUris { get; set; } = new();

    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public List<string> PostLogoutRedirectUris { get; set; } = new();
}
