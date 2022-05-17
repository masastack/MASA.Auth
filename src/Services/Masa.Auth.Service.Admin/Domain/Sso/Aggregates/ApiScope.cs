// Copyright (c) MASA Stack All rights reserved.
// Licensed under the Apache License. See LICENSE.txt in the project root for license information.

namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScope : FullAuditAggregateRoot<int, Guid>
{
    private List<ApiScopeClaim> _userClaims = new();
    private List<ApiScopeProperty> _properties = new();

    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; }

    public IReadOnlyCollection<ApiScopeClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<ApiScopeProperty> Properties => _properties;

    public ApiScope(string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public static implicit operator ApiScopeDetailDto(ApiScope apiScope)
    {
        var userClaims = apiScope.UserClaims.Select(userClaim => userClaim.UserClaimId).ToList();
        var properties = apiScope.Properties.ToDictionary(property => property.Key, property => property.Value);

        return new ApiScopeDetailDto(apiScope.Id, apiScope.Enabled, apiScope.Name, apiScope.DisplayName, apiScope.Description, apiScope.Required, apiScope.Emphasize, apiScope.ShowInDiscoveryDocument, userClaims, properties);
    }

    public void Update(string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public void BindUserClaims(List<int> userClaims)
    {
        _userClaims.Clear();
        _userClaims.AddRange(userClaims.Select(id => new ApiScopeClaim(id)));
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        //Todo add Properties;
    }
}
