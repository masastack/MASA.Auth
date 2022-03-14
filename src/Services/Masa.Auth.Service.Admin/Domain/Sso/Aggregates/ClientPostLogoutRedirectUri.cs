namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientPostLogoutRedirectUri : Entity<int>
{
    public string PostLogoutRedirectUri { get; set; } = "";

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
