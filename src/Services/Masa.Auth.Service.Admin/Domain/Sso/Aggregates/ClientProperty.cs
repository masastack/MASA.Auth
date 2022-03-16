namespace Masa.Auth.Service.Admin.Domain.Sso.Aggregates;

public class ClientProperty : Property
{
    public int ClientId { get; }

    public Client Client { get; } = null!;
}
