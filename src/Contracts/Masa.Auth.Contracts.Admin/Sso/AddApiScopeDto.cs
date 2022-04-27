namespace Masa.Auth.Contracts.Admin.Sso;

public class AddApiScopeDto
{
    public bool Enabled { get; set; }

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public List<int> UserClaims { get; private set; } = new();

    public Dictionary<string, string> Properties { get; private set; } = new();

    public AddApiScopeDto() { }

    public AddApiScopeDto(bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, List<int> userClaims, Dictionary<string, string> properties)
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

