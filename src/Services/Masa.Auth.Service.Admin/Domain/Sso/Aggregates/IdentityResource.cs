namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class IdentityResource : AggregateRoot<int>
{
    public bool Enabled { get; } = true;

    public string Name { get; } = "";

    public string DisplayName { get; } = "";

    public string Description { get; } = "";

    public bool Required { get; }

    public bool Emphasize { get; }

    public bool ShowInDiscoveryDocument { get; } = true;

    public List<IdentityResourceClaim> UserClaims { get; } = new();

    public List<IdentityResourceProperty> Properties { get; } = new();

    public DateTime Created { get; } = DateTime.UtcNow;

    public DateTime? Updated { get; }

    public bool NonEditable { get; }
}

