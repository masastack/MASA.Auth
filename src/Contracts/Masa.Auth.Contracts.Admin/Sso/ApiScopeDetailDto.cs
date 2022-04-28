namespace Masa.Auth.Contracts.Admin.Sso;

public class ApiScopeDetailDto : ApiScopeDto
{
    public List<int> UserClaims { get; private set; } = new();

    public Dictionary<string, string> Properties { get; private set; } = new();

    public ApiScopeDetailDto()
    {
    }

    public ApiScopeDetailDto(int id, bool enabled, string name, string displayName, string description, bool required, bool emphasize, bool showInDiscoveryDocument, List<int> userClaims, Dictionary<string, string> properties) : base(id, enabled, name, displayName, description, required, emphasize, showInDiscoveryDocument)
    {  
        UserClaims = userClaims;
        Properties = properties;
    }

    public static implicit operator UpdateApiScopeDto(ApiScopeDetailDto apiScope)
    {
        return new UpdateApiScopeDto();
    }
}

