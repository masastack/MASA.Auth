namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientProperty : Property
{
    public int ClientId { get; private set; }

    public Client Client { get; private set; } = null!;
}
