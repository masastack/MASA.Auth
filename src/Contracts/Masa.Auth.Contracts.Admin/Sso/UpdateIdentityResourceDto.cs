// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Contracts.Admin.Sso;

public class UpdateIdentityResourceDto
{
    public int Id { get; set; }

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Enabled { get; set; }

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; }

    public bool NonEditable { get; set; }

    public List<int> UserClaims { get; set; }

    public UpdateIdentityResourceDto()
    {
        UserClaims = new();
    }

    public UpdateIdentityResourceDto(int id, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable, List<int> userClaims)
    {
        Id = id;
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
        UserClaims = userClaims;
    }
}

