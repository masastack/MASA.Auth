namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientRedirectUri : Entity<int>
{
    public string RedirectUri { get; } = "";

    public int ClientId { get; }

    public Client Client { get; } = null!;
}

