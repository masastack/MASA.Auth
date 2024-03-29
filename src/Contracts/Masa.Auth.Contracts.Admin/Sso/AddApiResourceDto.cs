﻿// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class AddApiResourceDto
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; set; } = "";

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public DateTime? LastAccessed { get; set; }

    public bool NonEditable { get; set; }

    public List<Guid> ApiScopes { get; set; } = new();

    public List<Guid> UserClaims { get; set; } = new();

    public Dictionary<string, string> Properties { get; set; } = new();

    public List<string> Secrets { get; set; } = new();

    public AddApiResourceDto()
    {
    }

    public AddApiResourceDto(bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, List<Guid> apiScopes, List<Guid> userClaims, Dictionary<string, string> properties, List<string> secrets)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
        ApiScopes = apiScopes;
        UserClaims = userClaims;
        Properties = properties;
        Secrets = secrets;
    }
}

