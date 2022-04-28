namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiResource : AuditAggregateRoot<int, Guid>, ISoftDelete
{
    private List<ApiResourceSecret> _secrets = new();
    private List<ApiResourceScope> _apiScopes = new();
    private List<ApiResourceClaim> _userClaims = new();
    private List<ApiResourceProperty> _properties = new();

    public bool IsDeleted { get; private set; }

    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public string AllowedAccessTokenSigningAlgorithms { get; private set; } = "";

    public bool ShowInDiscoveryDocument { get; private set; } = true;

    public DateTime? LastAccessed { get; private set; }

    public bool NonEditable { get; private set; }

    public IReadOnlyCollection<ApiResourceSecret> Secrets => _secrets;

    public IReadOnlyCollection<ApiResourceScope> ApiScopes => _apiScopes;

    public IReadOnlyCollection<ApiResourceClaim> UserClaims => _userClaims;

    public IReadOnlyCollection<ApiResourceProperty> Properties => _properties;

    public ApiResource(string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
    }

    public static implicit operator ApiResourceDetailDto(ApiResource apiResource)
    {
        var apiScopes = apiResource.ApiScopes.Select(apiScope => apiScope.ApiScopeId).ToList();
        var userClaims = apiResource.UserClaims.Select(userClaim => userClaim.UserClaimId).ToList();
        var properties = apiResource.Properties.ToDictionary(property => property.Key, property => property.Value);
        var secrets = apiResource.Secrets.Select(secret => secret.Value).ToList();

        return new ApiResourceDetailDto(apiResource.Id, apiResource.Enabled, apiResource.Name, apiResource.DisplayName, apiResource.Description, apiResource.AllowedAccessTokenSigningAlgorithms, apiResource.ShowInDiscoveryDocument, apiResource.LastAccessed, apiResource.NonEditable, apiScopes, userClaims, properties, secrets);
    }

    public void Update(string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        AllowedAccessTokenSigningAlgorithms = allowedAccessTokenSigningAlgorithms;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
        LastAccessed = lastAccessed;
        NonEditable = nonEditable;
    }

    public void BindUserClaims(List<int> userClaims)
    {
        _userClaims.Clear();
        _userClaims.AddRange(userClaims.Select(id => new ApiResourceClaim(id)));
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        _properties.Clear();
        //Todo add Properties;
    }

    public void BindApiScopes(List<int> apiScopes)
    {
        _apiScopes.Clear();
        _apiScopes.AddRange(apiScopes.Select(id => new ApiResourceScope(id)));
    }
}

