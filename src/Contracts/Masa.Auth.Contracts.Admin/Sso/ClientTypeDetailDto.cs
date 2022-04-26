namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientTypeDetailDto
{
    public ClientTypes ClientType { get; set; }

    public ICollection<string> GrantTypes { get; set; } = new List<string>();

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}
