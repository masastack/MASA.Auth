namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScope : Entity<int>
{
    public bool Enabled { get; private set; } = true;

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public List<ApiScopeClaim> UserClaims { get; private set; } = new();

    public List<ApiScopeProperty> Properties { get; private set; } = new();
}
