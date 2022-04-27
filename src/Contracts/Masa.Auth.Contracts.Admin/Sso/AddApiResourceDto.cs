namespace Masa.Auth.Contracts.Admin.Sso;

public class AddApiResourceDto
{
    public bool Enabled { get; private set; } = true;

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; private set; } = "";

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public DateTime? LastAccessed { get; private set; }

    public bool NonEditable { get; private set; }

    public List<int> ApiScopes { get; private set; } = new();

    public List<int> UserClaims { get; private set; } = new();

    public Dictionary<string, string> Properties { get; private set; } = new();

    public List<int> Secrets { get; private set; } = new();

    public AddApiResourceDto()
    {
    }

    public AddApiResourceDto(bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, List<int> apiScopes, List<int> userClaims, Dictionary<string, string> properties, List<int> secrets)
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

