namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientCredentialDto
{
    public bool RequireClientSecret { get; set; }

    public List<ClientSecretDto> ClientSecrets { get; set; } = new();

    public ClientSecretDto ClientSecret { get; set; } = new();
}
