namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientBasicDto
{
    public Guid Id { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> GrantTypes { get; set; } = new();

    public bool RequirePkce { get; set; } = true;

    public bool Enabled { get; set; }

    public bool RequireRequestObject { get; set; }

    public string AllowedCorsOrigin { get; set; } = string.Empty;

    public List<string> AllowedCorsOrigins { get; set; } = new();

    public static implicit operator ClientBasicDto(ClientDetailDto cad)
    {
        return new ClientBasicDto()
        {
            ClientId = cad.ClientId,
            ClientName = cad.ClientName,
            Description = cad.Description
        };
    }
}
