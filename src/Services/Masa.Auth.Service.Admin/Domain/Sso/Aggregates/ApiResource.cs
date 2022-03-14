namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ApiResource : AggregateRoot<int>
{
    //https://github.com/IdentityServer/IdentityServer4/blob/3ff3b46698f48f164ab1b54d124125d63439f9d0/src/EntityFramework.Storage/src/Extensions/ModelBuilderExtensions.cs

    public bool Enabled { get; set; } = true;

    public string Name { get; set; } = "";

    public string DisplayName { get; set; } = "";

    public string Description { get; set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; set; } = "";

    public bool ShowInDiscoveryDocument { get; set; } = true;

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime? Updated { get; set; }

    public DateTime? LastAccessed { get; set; }

    public bool NonEditable { get; set; }

    public List<ApiResourceSecret> Secrets { get; set; } = new();

    public List<ApiResourceScope> Scopes { get; set; } = new();

    public List<ApiResourceClaim> UserClaims { get; set; } = new();

    public List<ApiResourceProperty> Properties { get; set; } = new();

}

