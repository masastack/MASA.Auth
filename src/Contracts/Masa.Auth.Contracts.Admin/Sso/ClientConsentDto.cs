namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientConsentDto
{
    public int Id { get; set; }

    public string ClientUri { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public bool RequireConsent { get; set; } = true;

    public bool AllowRememberConsent { get; set; }

    public int ConsentLifetime { get; set; }
}
