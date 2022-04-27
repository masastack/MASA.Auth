namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResource : AuditAggregateRoot<int, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public bool Enabled { get; private set; } = true;

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; private set; } = "";

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public DateTime? LastAccessed { get; private set; }

    public bool NonEditable { get; private set; }

    public List<ApiResourceSecret> Secrets { get; private set; } = new();

    public List<ApiResourceScope> ApiScopes { get; private set; } = new();

    public List<ApiResourceClaim> UserClaims { get; private set; } = new();

    public List<ApiResourceProperty> Properties { get; private set; } = new();

}

