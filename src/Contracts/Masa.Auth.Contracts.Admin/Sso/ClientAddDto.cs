namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientAddDto
{
    #region Basic
    public ClientTypes ClientType { get; set; }

    public string ClientId { get; set; } = string.Empty;

    public string ClientName { get; set; } = string.Empty;

    public string Description { get; set; } = string.Empty;

    public List<string> GrantTypes { get; set; } = new();

    public bool RequirePkce { get; set; }
    #endregion

    #region Authentication
    /// <summary>
    /// call back url
    /// </summary>
    public List<string> RedirectUris { get; set; } = new();

    /// <summary>
    /// logout url
    /// </summary>
    public List<string> PostLogoutRedirectUris { get; set; } = new();
    #endregion

    #region Consent Screen
    public string ClientUri { get; set; } = string.Empty;

    public string LogoUri { get; set; } = string.Empty;

    public bool RequireConsent { get; set; } = true;
    #endregion

    #region Token

    #endregion

    #region Device Flow
    public string UserCodeType { get; set; } = string.Empty;

    public int DeviceCodeLifetime { get; set; } = 300;
    #endregion

    #region Client Credentials

    #endregion
}
