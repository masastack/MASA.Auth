namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientPostLogoutRedirectUri : Entity<int>
{
    public string PostLogoutRedirectUri { get; private set; } = string.Empty;

    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;

    public ClientPostLogoutRedirectUri(string postLogoutRedirectUri)
    {
        PostLogoutRedirectUri = postLogoutRedirectUri;
    }
}
