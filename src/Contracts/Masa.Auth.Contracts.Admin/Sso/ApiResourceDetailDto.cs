namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiResourceDetailDto : ApiResourceDto
{
    public List<int> ApiScopes { get; set; } = new();

    public List<int> UserClaims { get; set; } = new();

    public Dictionary<string, string> Properties { get; set; } = new();

    public List<string> Secrets { get; set; } = new();

    public ApiResourceDetailDto() { }

    public ApiResourceDetailDto(int id, bool enabled, string name, string displayName, string description, string allowedAccessTokenSigningAlgorithms, bool showInDiscoveryDocument, DateTime? lastAccessed, bool nonEditable,List<int> apiScopes, List<int> userClaims, Dictionary<string, string> properties, List<string> secrets) : base(id, enabled, name, displayName, description, allowedAccessTokenSigningAlgorithms, showInDiscoveryDocument, lastAccessed, nonEditable)
    {
        ApiScopes = apiScopes;
        UserClaims = userClaims;
        Properties = properties;
        Secrets = secrets;
    }

    public static implicit operator UpdateApiResourceDto(ApiResourceDetailDto apiResource)
    {
        return new UpdateApiResourceDto(apiResource.Id, apiResource.Enabled, apiResource.Name, apiResource.DisplayName, apiResource.Description, apiResource.AllowedAccessTokenSigningAlgorithms, apiResource.ShowInDiscoveryDocument, apiResource.LastAccessed, apiResource.NonEditable, apiResource.ApiScopes, apiResource.UserClaims, apiResource.Properties, apiResource.Secrets);
    }
}

