namespace Masa.Auth.Contracts.Admin.Sso;

public class IdentityResourceDetailDto : IdentityResourceDto
{
    public List<int> UserClaims { get; set; }

    public Dictionary<string, string> Properties { get; set; }

    public IdentityResourceDetailDto()
    {
        UserClaims = new List<int>();
        Properties = new Dictionary<string, string>();
    }

    public IdentityResourceDetailDto(int id, string name, string displayName, string description, bool enabled, bool required, bool emphasize, bool showInDiscoveryDocument, bool nonEditable,List<int> userClaims, Dictionary<string, string> properties) : base(id, name, displayName, description, enabled, required, emphasize, showInDiscoveryDocument, nonEditable)
    {
        UserClaims = userClaims;
        Properties = properties;
    }
}

