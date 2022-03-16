namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResource : AggregateRoot<int>
{
    //https://github.com/IdentityServer/IdentityServer4/blob/3ff3b46698f48f164ab1b54d124125d63439f9d0/src/EntityFramework.Storage/src/Extensions/ModelBuilderExtensions.cs

    public bool Enabled { get; } = true;

    public string Name { get; } = "";

    public string DisplayName { get; } = "";

    public string Description { get; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; } = "";

    public bool ShowInDiscoveryDocument { get; } = true;

    public DateTime Created { get; } = DateTime.UtcNow;

    public DateTime? Updated { get; }

    public DateTime? LastAccessed { get; }

    public bool NonEditable { get; }

    public List<ApiResourceSecret> Secrets { get; } = new();

    public List<ApiResourceScope> Scopes { get; } = new();

    public List<ApiResourceClaim> UserClaims { get; } = new();

    public List<ApiResourceProperty> Properties { get; } = new();

}

