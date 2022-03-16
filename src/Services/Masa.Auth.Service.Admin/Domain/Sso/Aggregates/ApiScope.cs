namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScope : Entity<int>
{
    public bool Enabled { get; } = true;

    public string Name { get; } = "";

    public string DisplayName { get; } = "";

    public string Description { get; } = "";

    public bool Required { get; }

    public bool Emphasize { get; }

    public bool ShowInDiscoveryDocument { get; } = true;

    public List<ApiScopeClaim> UserClaims { get; } = new();

    public List<ApiScopeProperty> Properties { get; } = new();
}
