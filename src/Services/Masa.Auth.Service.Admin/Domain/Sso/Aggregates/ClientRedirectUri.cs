namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientRedirectUri : Entity<int>
{
    public string RedirectUri { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientRedirectUri(string redirectUri)
    {
        RedirectUri = redirectUri;
    }
}

