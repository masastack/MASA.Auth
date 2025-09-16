// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class IdentityResourceDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; }

    public bool Required { get; set; }

    public string Type { get; set; } = "";

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; }

    public bool NonEditable { get; set; }

    public IdentityResourceDto()
    {

    }

    public IdentityResourceDto(Guid id, string name, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable)
    {
        Id = id;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
    }
}

