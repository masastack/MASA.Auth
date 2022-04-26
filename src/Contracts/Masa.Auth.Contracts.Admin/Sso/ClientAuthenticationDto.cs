namespace Masa.Auth.Contracts.Admin.Sso;

public class ClientAuthenticationDto
{
    public Guid Id { get; set; }

    public string RedirectUri { get; set; } = string.Empty;

    public List<string> RedirectUris { get; set; } = new();

    public string PostLogoutRedirectUri { get; set; } = string.Empty;

    public List<string> PostLogoutRedirectUris { get; set; } = new();

    public string FrontChannelLogoutUri { get; set; } = string.Empty;

    public bool FrontChannelLogoutSessionRequired { get; set; }

    public string BackChannelLogoutUri { get; set; } = string.Empty;

    public bool BackChannelLogoutSessionRequired { get; set; }

    public bool EnableLocalLogin { get; set; }

    public string IdentityProviderRestriction { get; set; } = string.Empty;

    public List<string> IdentityProviderRestrictions { get; set; } = new();

    public int UserSsoLifetime { get; set; }
}
