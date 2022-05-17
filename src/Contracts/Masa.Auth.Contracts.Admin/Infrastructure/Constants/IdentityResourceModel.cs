// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Infrastructure.Constants;

public class IdentityResourceModel
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public List<string> UserClaims { get; set; } = new();

    public Dictionary<string, string> Properties { get; set; } = new();

    public bool Required { get; set; }

    public bool Emphasize { get; set; }
}

