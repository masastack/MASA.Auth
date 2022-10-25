// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiScopeDto
{
    public Guid Id { get; set; }

    public bool Enabled { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public ApiScopeDto()
    {

    }

    public ApiScopeDto(Guid id, bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument)
    {
        Id = id;
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }
}

