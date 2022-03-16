namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResource : AggregateRoot<int>
{
    public bool Enabled { get; private set; } = true;

    public string Name { get; private set; } = string.Empty;

    public string DisplayName { get; private set; } = string.Empty;

    public string Description { get; private set; } = string.Empty;

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public List<IdentityResourceClaim> UserClaims { get; private set; } = new();

    public List<IdentityResourceProperty> Properties { get; private set; } = new();

    public DateTime Created { get; private set; } = DateTime.UtcNow;

    public DateTime? Updated { get; private set; }

    public bool NonEditable { get; private set; }
}

