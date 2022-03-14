namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientRedirectUri : Entity<int>
{
    public string RedirectUri { get; set; } = "";

    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}

