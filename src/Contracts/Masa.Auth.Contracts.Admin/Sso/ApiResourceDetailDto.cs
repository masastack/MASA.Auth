namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiResourceDetailDto : ApiResourceDto
{
    public List<int> ApiScopes { get; private set; } = new();

    public List<int> UserClaims { get; private set; } = new();

    public Dictionary<string, string> Properties { get; private set; } = new();

    public List<int> Secrets { get; private set; } = new();

    public ApiResourceDetailDto() { }

    public ApiResourceDetailDto(int id, bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable,List<int> apiScopes, List<int> userClaims, Dictionary<string, string> properties, List<int> secrets) : base(id, enabled, name, displayName, description, allowedAccessTokenSigningAlgorithms, showInDiscoveryDocument, lastAccessed, nonEditable)
    {
        ApiScopes = apiScopes;
        UserClaims = userClaims;
        Properties = properties;
        Secrets = secrets;
    }
}

