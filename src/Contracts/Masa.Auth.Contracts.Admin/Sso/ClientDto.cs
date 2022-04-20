namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientDto
{
    public int Id { get; set; }

    public string ClientName { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public string ClientId { get; set; } = string.Empty;

    public bool Enabled { get; set; }

    public ClientTypes ClientType { get; set; }
}
