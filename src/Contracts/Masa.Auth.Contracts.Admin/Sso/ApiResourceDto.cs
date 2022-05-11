// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiResourceDto
{
    public int Id { get; set; }

    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; set; } = "";

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public DateTime? LastAccessed { get; set; }

    public bool NonEditable { get; set; }

    public ApiResourceDto() { }

    public ApiResourceDto(int id, bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable)
    {
        Id = id;
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
    }
}

