namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class IdentityResource : AggregateRoot<int>
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public List<IdentityResourceClaim> UserClaims { get; set; } = new();

    public List<IdentityResourceProperty> Properties { get; set; } = new();

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Updated { get; set; }

    public bool NonEditable { get; set; }
}

