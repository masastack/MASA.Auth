namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiResourceDto
{
    public int Id { get; set; }

    public bool Enabled { get; private set; } = true;

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; private set; } = "";

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public DateTime? LastAccessed { get; private set; }

    public bool NonEditable { get; private set; }

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

