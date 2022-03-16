namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientPostLogoutRedirectUri : Entity<int>
{
    public string PostLogoutRedirectUri { get; } = "";

    public int ClientId { get; }

    public Client Client { get; } = null!;
}
