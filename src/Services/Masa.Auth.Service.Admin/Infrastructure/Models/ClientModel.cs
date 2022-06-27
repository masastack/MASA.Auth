// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Infrastructure.Models;

public class ClientModel
{
    public int ClientType { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public List<string> AllowedScopes { get; set; } = new();

    public List<string> RedirectUris { get; set; } = new();

    public List<string> PostLogoutRedirectUris { get; set; } = new();
}
