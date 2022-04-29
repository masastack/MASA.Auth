// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResource : AuditAggregateRoot<int, Guid>, ISoftDelete
{
    private List<IdentityResourceClaim> _userClaims = new();
    private List<IdentityResourceProperty> _properties = new();

    public bool IsDeleted { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool Enabled { get; private set; } = true;

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public IReadOnlyCollection<IdentityResourceClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<IdentityResourceProperty> Properties => _properties;

    public bool NonEditable { get; private set; }

    public IdentityResource(string name, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable)
    {
        Name = name;
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
    }

    public void BindUserClaims(List<int> userClaims)
    {
        _userClaims.Clear();
        _userClaims.AddRange(userClaims.Select(id => new IdentityResourceClaim(id)));
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        //Todo add Properties;
    }

    public void Update(string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable)
    {
        DisplayName = displayName;
        Description = description;
        Enabled = enabled;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        NonEditable = nonEditable;
    }
}

