namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientSecret : Secret
{
    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
