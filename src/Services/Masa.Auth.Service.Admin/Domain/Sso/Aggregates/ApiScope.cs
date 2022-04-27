namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScope : Entity<int>
{
    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; }

    public List<ApiScopeClaim> UserClaims { get; private set; } = new();

    public List<ApiScopeProperty> Properties { get; private set; } = new();

    public ApiScope(bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, List<ApiScopeClaim> userClaims, List<ApiScopeProperty> properties)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        UserClaims = userClaims;
        Properties = properties;
    }
}
