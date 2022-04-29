namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientSecretDto
{
    public int Id { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public DateTime? Expiration { get; set; }

    public string Type { get; set; } = "SharedSecret";
}
