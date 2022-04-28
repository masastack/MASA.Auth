namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ApiScope : AuditAggregateRoot<int, Guid>, ISoftDelete
{
    public bool IsDeleted { get; private set; }

    public bool Enabled { get; private set; }

    public string Name { get; private set; } = "";

    public string DisplayName { get; private set; } = "";

    public string Description { get; private set; } = "";

    public bool Required { get; private set; }

    public bool Emphasize { get; private set; }

    public bool ShowInDiscoveryDocument { get; private set; }

    public List<ApiScopeClaim> UserClaims { get; private set; } = new();

    public List<ApiScopeProperty> Properties { get; private set; } = new();  

    public ApiScope(string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public static implicit operator ApiScopeDetailDto(ApiScope apiScope)
    {
        var userClaims = apiScope.UserClaims.Select(userClaim => userClaim.UserClaimId).ToList();
        var properties = apiScope.Properties.ToDictionary(property => property.Key, property => property.Value);        

        return new ApiScopeDetailDto(apiScope.Id, apiScope.Enabled, apiScope.Name, apiScope.DisplayName, apiScope.Description, apiScope.Required, apiScope.Emphasize, apiScope.ShowInDiscoveryDocument, userClaims, properties);
    }

    public void Update(string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, bool enabled)
    {
        Enabled = enabled;
        Name = name;
        DisplayName = displayName;
        Description = description;
        Required = required;
        Emphasize = emphasize;
        ShowInDiscoveryDocument = showInDiscoveryDocument;
    }

    public void BindUserClaims(List<int> userClaims)
    {
        UserClaims.Clear();
        UserClaims.AddRange(userClaims.Select(id => new ApiScopeClaim(id)));
    }

    public void BindProperties(Dictionary<string, string> properties)
    {
        Properties.Clear();
        //Todo add Properties;
    }
}
