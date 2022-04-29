namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientTypeDetailDto
{
    public ClientTypes ClientType { get; set; }

    public string Description { get; set; } = string.Empty;

    public string Icon { get; set; } = string.Empty;
}
