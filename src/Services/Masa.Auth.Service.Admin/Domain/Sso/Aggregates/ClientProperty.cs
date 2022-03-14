namespace Masa.Auth.Service.Domain.Sso.Aggregates;

public class ClientProperty : Property
{
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;
}
