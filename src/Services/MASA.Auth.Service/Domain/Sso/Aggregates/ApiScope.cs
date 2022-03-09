namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiScope : Entity<int>
{
    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public bool Required { get; set; }

    public bool Emphasize { get; set; }

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public List<ApiScopeClaim> UserClaims { get; set; } = new();

    public List<ApiScopeProperty> Properties { get; set; } = new();
}
