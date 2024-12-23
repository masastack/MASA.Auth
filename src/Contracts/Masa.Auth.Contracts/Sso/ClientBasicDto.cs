// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientBasicDto
{
    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> GrantTypes { get; set; } = new();

    public bool RequirePkce { get; set; } = true;

    public bool Enabled { get; set; }

    public bool RequireRequestObject { get; set; }

    public string AllowedCorsOrigin { get; set; } = string.Empty;

    public List<string> AllowedCorsOrigins { get; set; } = new();

    public List<ClientPropertyDto> Properties { get; set; } = new();

    public ClientPropertyDto Property { get; set; } = new();
}
