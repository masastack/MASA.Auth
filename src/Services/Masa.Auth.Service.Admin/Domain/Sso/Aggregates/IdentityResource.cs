namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResource : AuditAggregateRoot<int, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool Enabled { get; private set; } = true;

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public List<IdentityResourceClaim> UserClaims { get; private set; } = new();

    public List<IdentityResourceProperty> Properties { get; private set; } = new();

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
        UserClaims.Clear();
        //Todo add UserClaims;
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        Properties.Clear();
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

